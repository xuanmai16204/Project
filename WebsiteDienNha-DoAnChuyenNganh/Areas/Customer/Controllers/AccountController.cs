using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.Models;
using WebsiteDienNha_DoAnChuyenNganh.Services;

namespace WebsiteDienNha_DoAnChuyenNganh.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class AccountController : Controller
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;
		private readonly SmsService _smsService;
		private readonly OtpService _otpService;

		public AccountController(
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
			ApplicationDbContext context,
			SmsService smsService,
			OtpService otpService)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_context = context;
			_smsService = smsService;
			_otpService = otpService;
		}

		[HttpGet]
		public IActionResult Login(string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Find user by phone number
			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);

			if (user == null)
			{
				ModelState.AddModelError(string.Empty, "Số điện thoại hoặc mật khẩu không đúng.");
				return View(model);
			}

			// Verify password directly
			var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
			if (!passwordValid)
			{
				var hasPasswordHash = await _userManager.HasPasswordAsync(user);
				if (!hasPasswordHash)
				{
					var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
					var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, model.Password);
					if (resetResult.Succeeded)
					{
						await _signInManager.SignInAsync(user, model.RememberMe);
						return RedirectToLocal(returnUrl);
					}
				}

				ModelState.AddModelError(string.Empty, "Số điện thoại hoặc mật khẩu không đúng.");
				return View(model);
			}

			// Check if user is locked out
			if (await _userManager.IsLockedOutAsync(user))
			{
				ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng thử lại sau.");
				return View(model);
			}

			// Sign in the user
			await _signInManager.SignInAsync(user, model.RememberMe);

			// Redirect admin to admin dashboard
			if (await _userManager.IsInRoleAsync(user, SD.RoleAdmin))
			{
				return RedirectToAction("Index", "Home", new { area = "Admin" });
			}

			return RedirectToLocal(returnUrl);
		}

		[HttpGet]
		public IActionResult Register(string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// OTP là tùy chọn - nếu có OTP thì verify, nếu không có thì bỏ qua
			if (!string.IsNullOrEmpty(model.OtpCode))
			{
				if (!_otpService.VerifyOtp(model.PhoneNumber, model.OtpCode))
				{
					ModelState.AddModelError(nameof(model.OtpCode), "Mã OTP không đúng hoặc đã hết hạn. Vui lòng thử lại hoặc bỏ qua bước này.");
					ViewData["ShowOtpSection"] = true;
					ViewData["PhoneNumber"] = model.PhoneNumber;
					_otpService.IncrementAttempts(model.PhoneNumber);
					return View(model);
				}
			}

			// Check if phone number already exists
			var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber);
			if (existingUser != null)
			{
				ModelState.AddModelError(nameof(model.PhoneNumber), "Số điện thoại này đã được sử dụng.");
				ViewData["ShowOtpSection"] = true;
				return View(model);
			}

			// Check if username (phone number) already exists
			var existingUserName = await _userManager.FindByNameAsync(model.PhoneNumber);
			if (existingUserName != null)
			{
				ModelState.AddModelError(nameof(model.PhoneNumber), "Số điện thoại này đã được sử dụng.");
				ViewData["ShowOtpSection"] = true;
				return View(model);
			}

			// Create user with phone number as username
			var user = new ApplicationUser
			{
				UserName = model.PhoneNumber,
				PhoneNumber = model.PhoneNumber,
				PhoneNumberConfirmed = !string.IsNullOrEmpty(model.OtpCode), // Chỉ confirm nếu có OTP
				Email = model.Email,
				EmailConfirmed = false
			};
			var result = await _userManager.CreateAsync(user, model.Password);
			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(user, SD.RoleCustomer);
				if (!string.IsNullOrEmpty(model.OtpCode))
				{
					_otpService.ResetAttempts(model.PhoneNumber);
				}
				await _signInManager.SignInAsync(user, isPersistent: false);
				return RedirectToLocal(returnUrl);
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			ViewData["ShowOtpSection"] = true;
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> SendOtp(string phoneNumber)
		{
			if (string.IsNullOrEmpty(phoneNumber))
			{
				return Json(new { success = false, message = "Vui lòng nhập số điện thoại." });
			}

			// Kiểm tra số điện thoại đã tồn tại chưa
			var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
			if (existingUser != null)
			{
				return Json(new { success = false, message = "Số điện thoại này đã được sử dụng." });
			}

			// Kiểm tra rate limiting
			var remainingAttempts = _otpService.GetRemainingAttempts(phoneNumber);
			if (remainingAttempts <= 0)
			{
				return Json(new { success = false, message = "Bạn đã gửi quá nhiều OTP. Vui lòng thử lại sau 15 phút." });
			}

			// Tạo và gửi OTP
			var otp = _otpService.GenerateOtp();
			_otpService.StoreOtp(phoneNumber, otp);

			var smsSent = await _smsService.SendOtpAsync(phoneNumber, otp);

			if (smsSent)
			{
				return Json(new { success = true, message = "Mã OTP đã được gửi đến số điện thoại của bạn." });
			}
			else
			{
				return Json(new { success = false, message = "Không thể gửi OTP. Vui lòng thử lại sau." });
			}
		}

		[HttpPost]
		public IActionResult VerifyOtp(string phoneNumber, string otpCode)
		{
			if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(otpCode))
			{
				return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin." });
			}

			if (_otpService.VerifyOtp(phoneNumber, otpCode))
			{
				_otpService.ResetAttempts(phoneNumber);
				return Json(new { success = true, message = "Xác thực OTP thành công." });
			}
			else
			{
				_otpService.IncrementAttempts(phoneNumber);
				var remaining = _otpService.GetRemainingAttempts(phoneNumber);
				return Json(new { success = false, message = $"Mã OTP không đúng. Còn {remaining} lần thử." });
			}
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home", new { area = "" });
		}

		private IActionResult RedirectToLocal(string? returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home", new { area = "" });
		}
	}

	public class LoginViewModel
	{
		[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
		[System.ComponentModel.DataAnnotations.Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
		[System.ComponentModel.DataAnnotations.Display(Name = "Số điện thoại")]
		public string PhoneNumber { get; set; } = string.Empty;

		[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		[System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
		public string Password { get; set; } = string.Empty;

		public bool RememberMe { get; set; }
	}

	public class RegisterViewModel
	{
		[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
		[System.ComponentModel.DataAnnotations.Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
		[System.ComponentModel.DataAnnotations.Display(Name = "Số điện thoại")]
		public string PhoneNumber { get; set; } = string.Empty;

		[System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Email không hợp lệ")]
		[System.ComponentModel.DataAnnotations.Display(Name = "Email (tùy chọn)")]
		public string? Email { get; set; }

		[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		[System.ComponentModel.DataAnnotations.StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 6)]
		[System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
		[System.ComponentModel.DataAnnotations.Display(Name = "Xác nhận mật khẩu")]
		[System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp.")]
		public string ConfirmPassword { get; set; } = string.Empty;

		[System.ComponentModel.DataAnnotations.Display(Name = "Mã OTP (tùy chọn)")]
		[System.ComponentModel.DataAnnotations.StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải có 6 chữ số")]
		public string? OtpCode { get; set; }
	}
}

