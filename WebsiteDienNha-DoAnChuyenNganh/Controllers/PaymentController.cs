using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebsiteDienNha_DoAnChuyenNganh.Config;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.IRepository;
using WebsiteDienNha_DoAnChuyenNganh.Models;
using WebsiteDienNha_DoAnChuyenNganh.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Controllers
{
	[Authorize]
	public class PaymentController : Controller
	{
		private readonly IOrderRepository _orderRepository;
		private readonly ApplicationDbContext _context;
		private readonly VietQRService _vietQRService;
		private readonly UserManager<ApplicationUser> _userManager;

		public PaymentController(
			IOrderRepository orderRepository,
			ApplicationDbContext context,
			VietQRService vietQRService,
			UserManager<ApplicationUser> userManager)
		{
			_orderRepository = orderRepository;
			_context = context;
			_vietQRService = vietQRService;
			_userManager = userManager;
		}

		private async Task<IActionResult> CheckAdminAndRedirectAsync()
		{
			if (User.Identity?.IsAuthenticated == true)
			{
				var user = await _userManager.GetUserAsync(User);
				if (user != null && await _userManager.IsInRoleAsync(user, SD.RoleAdmin))
				{
					TempData["ErrorMessage"] = "Admin không thể thanh toán đơn hàng.";
					return RedirectToAction("Index", "Home", new { area = "Admin" });
				}
			}
			return null!;
		}

		[HttpGet]
		public async Task<IActionResult> Confirm(int orderId)
		{
			var adminCheck = await CheckAdminAndRedirectAsync();
			if (adminCheck != null) return adminCheck;

			var order = await _orderRepository.GetByIdAsync(orderId);
			if (order == null)
			{
				return NotFound();
			}

			// Kiểm tra nếu đơn hàng đã được thanh toán
			if (order.Status == "Paid")
			{
				TempData["PaymentSuccess"] = true;
				TempData["OrderId"] = orderId;
				return RedirectToAction("OrderCompleted", "ShoppingCart", new { area = "Customer", orderId = orderId });
			}

			// Hiển thị trang xác nhận thanh toán với QR code
			ViewBag.OrderId = orderId;
			ViewBag.Amount = order.Total;
			ViewBag.QRCodeUrl = _vietQRService.GenerateQRCodeUrl(order.Total, $"Don hang #{orderId}");
			ViewBag.BankInfo = _vietQRService.GetBankInfo();
			ViewBag.Deeplink = _vietQRService.GenerateDeeplink(order.Total, "Thanh toan don hang", orderId);

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
		{
			var adminCheck = await CheckAdminAndRedirectAsync();
			if (adminCheck != null) return adminCheck;

			if (request == null || request.OrderId <= 0)
			{
				return Json(new { success = false, message = "Thông tin không hợp lệ." });
			}

			var order = await _orderRepository.GetByIdAsync(request.OrderId);
			if (order == null)
			{
				return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
			}

			// Cập nhật trạng thái đơn hàng thành đã thanh toán
			// Lưu ý: Trong thực tế, bạn nên tích hợp với Payment Confirmation API từ VietQR
			// hoặc sử dụng webhook để xác nhận thanh toán tự động
			order.Status = "Paid";
			await _orderRepository.UpdateAsync(order);

			TempData["PaymentSuccess"] = true;
			TempData["OrderId"] = request.OrderId;

			return Json(new { success = true, message = "Xác nhận thanh toán thành công!", redirectUrl = Url.Action("OrderCompleted", "ShoppingCart", new { area = "Customer", orderId = request.OrderId }) });
		}
	}

	public class ConfirmPaymentRequest
	{
		public int OrderId { get; set; }
	}
}

