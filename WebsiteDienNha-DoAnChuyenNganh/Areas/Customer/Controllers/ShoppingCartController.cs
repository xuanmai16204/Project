using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.DTO;
using WebsiteDienNha_DoAnChuyenNganh.IRepository;
using WebsiteDienNha_DoAnChuyenNganh.Models;
using WebsiteDienNha_DoAnChuyenNganh.Services;

namespace WebsiteDienNha_DoAnChuyenNganh.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class ShoppingCartController : Controller
	{
		private readonly IProductRepository _productRepository;
		private readonly IOrderRepository _orderRepository;
		private readonly CartService _cartService;
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly VietQRService _vietQRService;

		public ShoppingCartController(
			IProductRepository productRepository,
			IOrderRepository orderRepository,
			CartService cartService,
			ApplicationDbContext context,
			UserManager<ApplicationUser> userManager,
			VietQRService vietQRService)
		{
			_productRepository = productRepository;
			_orderRepository = orderRepository;
			_cartService = cartService;
			_context = context;
			_userManager = userManager;
			_vietQRService = vietQRService;
		}

		private async Task<IActionResult> CheckAdminAndRedirectAsync()
		{
			if (User.Identity?.IsAuthenticated == true)
			{
				var user = await _userManager.GetUserAsync(User);
				if (user != null && await _userManager.IsInRoleAsync(user, SD.RoleAdmin))
				{
					TempData["ErrorMessage"] = "Admin không thể sử dụng chức năng đặt hàng.";
					return RedirectToAction("Index", "Home", new { area = "Admin" });
				}
			}
			return null!;
		}

		public async Task<IActionResult> Index()
		{
			var adminCheck = await CheckAdminAndRedirectAsync();
			if (adminCheck != null) return adminCheck;

			var items = _cartService.GetCart();
			var viewModel = new List<(CartItem Item, Product? Product)>();
			foreach (var item in items)
			{
				var product = await _productRepository.GetByIdAsync(item.ProductId);
				viewModel.Add((item, product));
			}
			ViewBag.CartItems = viewModel;

			// Load danh sách mã giảm giá đang còn hiệu lực
			var now = DateTime.UtcNow;
			var cartTotal = items.Sum(i => i.UnitPrice * i.Quantity);
			var availableDiscountCodes = await _context.DiscountCodes
				.Where(dc => dc.IsActive 
					&& dc.StartDate <= now 
					&& dc.EndDate >= now
					&& (dc.UsageLimit == 0 || dc.UsageCount < dc.UsageLimit)
					&& (!dc.MinimumOrderAmount.HasValue || dc.MinimumOrderAmount.Value <= cartTotal))
				.OrderByDescending(dc => dc.DiscountPercent ?? dc.DiscountAmount ?? 0)
				.ToListAsync();
			
			ViewBag.AvailableDiscountCodes = availableDiscountCodes;

			// Load discount code đã áp dụng từ session
			decimal discountAmount = 0;
			string? appliedDiscountCode = null;
			int? appliedDiscountCodeId = null;
			decimal? discountPercent = null;
			decimal? discountFixed = null;

			var discountCodeStr = HttpContext.Session.GetString("AppliedDiscountCode");
			if (!string.IsNullOrEmpty(discountCodeStr))
			{
				var discountCodeIdStr = HttpContext.Session.GetString("DiscountCodeId");
				if (!string.IsNullOrEmpty(discountCodeIdStr) && int.TryParse(discountCodeIdStr, out var discountCodeId))
				{
					var discountCode = await _context.DiscountCodes.FindAsync(discountCodeId);
					if (discountCode != null && discountCode.IsActive)
					{
						var currentTime = DateTime.UtcNow;
						if (currentTime >= discountCode.StartDate && currentTime <= discountCode.EndDate)
						{
							// Kiểm tra lại điều kiện đơn tối thiểu
							if (!discountCode.MinimumOrderAmount.HasValue || cartTotal >= discountCode.MinimumOrderAmount.Value)
							{
								// Tính discount
								if (discountCode.DiscountPercent.HasValue)
								{
									discountAmount = cartTotal * (discountCode.DiscountPercent.Value / 100);
									discountPercent = discountCode.DiscountPercent.Value;
								}
								else if (discountCode.DiscountAmount.HasValue)
								{
									discountAmount = discountCode.DiscountAmount.Value;
									discountFixed = discountCode.DiscountAmount.Value;
								}

								// Đảm bảo discount không vượt quá cart total
								discountAmount = Math.Min(discountAmount, cartTotal);
								appliedDiscountCode = discountCode.Code;
								appliedDiscountCodeId = discountCode.Id;
							}
							else
							{
								// Đơn hàng không đủ điều kiện, xóa discount
								HttpContext.Session.Remove("AppliedDiscountCode");
								HttpContext.Session.Remove("DiscountCodeId");
							}
						}
						else
						{
							// Mã đã hết hạn, xóa discount
							HttpContext.Session.Remove("AppliedDiscountCode");
							HttpContext.Session.Remove("DiscountCodeId");
						}
					}
					else
					{
						// Mã không tồn tại hoặc đã bị vô hiệu, xóa discount
						HttpContext.Session.Remove("AppliedDiscountCode");
						HttpContext.Session.Remove("DiscountCodeId");
					}
				}
			}

			var finalTotal = Math.Max(0, cartTotal - discountAmount);

			ViewBag.CartTotal = cartTotal;
			ViewBag.DiscountAmount = discountAmount;
			ViewBag.AppliedDiscountCode = appliedDiscountCode;
			ViewBag.AppliedDiscountCodeId = appliedDiscountCodeId;
			ViewBag.DiscountPercent = discountPercent;
			ViewBag.DiscountFixed = discountFixed;
			ViewBag.FinalTotal = finalTotal;

			return View(items);
		}

		[HttpPost]
		[IgnoreAntiforgeryToken]
		public async Task<IActionResult> Add(int productId, int quantity = 1)
		{
			var adminCheck = await CheckAdminAndRedirectAsync();
			if (adminCheck != null) return adminCheck;

			var product = await _productRepository.GetByIdAsync(productId);
			if (product == null) return NotFound();
			
			// Check stock
			if (product.Stock < quantity)
			{
				if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
				{
					return Json(new { success = false, message = "Sản phẩm không đủ hàng" });
				}
				TempData["ErrorMessage"] = "Sản phẩm không đủ hàng";
				return RedirectToAction("Index", "Product");
			}
			
			var cart = _cartService.GetCart();
			var existing = cart.FirstOrDefault(i => i.ProductId == productId);
			
			// Sử dụng giá khuyến mãi nếu có, nếu không thì dùng giá gốc
			var unitPrice = WebsiteDienNha_DoAnChuyenNganh.Utils.ProductHelper.GetPromotionPrice(product);
			
			if (existing == null)
			{
				cart.Add(new CartItem
				{
					ProductId = product.Id,
					Quantity = Math.Max(1, quantity),
					UnitPrice = unitPrice
				});
			}
			else
			{
				// Cập nhật giá nếu giá khuyến mãi thay đổi
				existing.UnitPrice = unitPrice;
				existing.Quantity += Math.Max(1, quantity);
			}
			_cartService.SaveCart(cart);
			
			// Return JSON for AJAX requests
			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				var cartCount = cart.Sum(i => i.Quantity);
				return Json(new { success = true, message = "Đã thêm sản phẩm vào giỏ hàng", count = cartCount });
			}
			
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[IgnoreAntiforgeryToken]
		public async Task<IActionResult> Update(int productId, int quantity)
		{
			var cart = _cartService.GetCart();
			var item = cart.FirstOrDefault(i => i.ProductId == productId);
			if (item == null) return NotFound();
			
			// Cập nhật giá khuyến mãi nếu có thay đổi
			var product = await _productRepository.GetByIdAsync(productId);
			if (product != null)
			{
				var promotionPrice = WebsiteDienNha_DoAnChuyenNganh.Utils.ProductHelper.GetPromotionPrice(product);
				item.UnitPrice = promotionPrice;
			}
			
			item.Quantity = Math.Max(1, quantity);
			_cartService.SaveCart(cart);
			
			// Return JSON for AJAX requests, otherwise redirect
			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				var total = cart.Sum(i => i.UnitPrice * i.Quantity);
				var itemTotal = item.UnitPrice * item.Quantity;
				
				// Check available discount codes based on new total
				var now = DateTime.UtcNow;
				var availableDiscountCodes = await _context.DiscountCodes
					.Where(dc => dc.IsActive 
						&& dc.StartDate <= now 
						&& dc.EndDate >= now
						&& (dc.UsageLimit == 0 || dc.UsageCount < dc.UsageLimit)
						&& (!dc.MinimumOrderAmount.HasValue || dc.MinimumOrderAmount.Value <= total))
					.OrderByDescending(dc => dc.DiscountPercent ?? dc.DiscountAmount ?? 0)
					.Select(dc => new {
						dc.Id,
						dc.Code,
						dc.Description,
						dc.DiscountPercent,
						dc.DiscountAmount,
						dc.MinimumOrderAmount,
						DiscountText = dc.DiscountPercent.HasValue 
							? $"Giảm {dc.DiscountPercent.Value}%" 
							: (dc.DiscountAmount.HasValue ? $"Giảm {dc.DiscountAmount.Value.ToString("N0")} đ" : "Không có giảm giá")
					})
					.ToListAsync();
				
				// Check if there's a better discount code available
				var currentDiscountCodeIdStr = HttpContext.Session.GetString("DiscountCodeId");
				int? currentDiscountCodeId = null;
				if (!string.IsNullOrEmpty(currentDiscountCodeIdStr) && int.TryParse(currentDiscountCodeIdStr, out var codeId))
				{
					currentDiscountCodeId = codeId;
				}
				
				var betterDiscountCodes = availableDiscountCodes
					.Where(dc => !currentDiscountCodeId.HasValue || dc.Id != currentDiscountCodeId.Value)
					.ToList();
				
				return Json(new { 
					success = true, 
					quantity = item.Quantity,
					itemTotal = itemTotal,
					total = total,
					availableDiscountCodes = betterDiscountCodes,
					hasBetterDiscount = betterDiscountCodes.Any()
				});
			}
			
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public IActionResult Remove(int productId)
		{
			var cart = _cartService.GetCart();
			cart.RemoveAll(i => i.ProductId == productId);
			_cartService.SaveCart(cart);
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public IActionResult Clear()
		{
			_cartService.Clear();
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public IActionResult GetCount()
		{
			var cart = _cartService.GetCart();
			var count = cart.Sum(i => i.Quantity);
			return Json(new { count = count });
		}

		[HttpGet]
		[IgnoreAntiforgeryToken]
		public async Task<IActionResult> GetAvailableDiscountCodes(decimal? cartTotal = null)
		{
			// Nếu không có cartTotal, tính từ giỏ hàng
			if (!cartTotal.HasValue)
			{
				var cart = _cartService.GetCart();
				cartTotal = cart.Sum(i => i.UnitPrice * i.Quantity);
			}

			var now = DateTime.UtcNow;
			var availableDiscountCodes = await _context.DiscountCodes
				.Where(dc => dc.IsActive 
					&& dc.StartDate <= now 
					&& dc.EndDate >= now
					&& (dc.UsageLimit == 0 || dc.UsageCount < dc.UsageLimit)
					&& (!dc.MinimumOrderAmount.HasValue || dc.MinimumOrderAmount.Value <= cartTotal.Value))
				.OrderByDescending(dc => dc.DiscountPercent ?? dc.DiscountAmount ?? 0)
				.Select(dc => new {
					dc.Id,
					dc.Code,
					dc.Description,
					dc.DiscountPercent,
					dc.DiscountAmount,
					dc.MinimumOrderAmount,
					EndDate = dc.EndDate,
					DiscountText = dc.DiscountPercent.HasValue 
						? $"Giảm {dc.DiscountPercent.Value}%" 
						: (dc.DiscountAmount.HasValue ? $"Giảm {dc.DiscountAmount.Value.ToString("N0")} đ" : "Không có giảm giá"),
					EstimatedSavings = dc.DiscountPercent.HasValue 
						? (cartTotal.Value * dc.DiscountPercent.Value / 100)
						: (dc.DiscountAmount ?? 0)
				})
				.ToListAsync();

			// Check if user already applied a discount code
			var currentDiscountCodeIdStr = HttpContext.Session.GetString("DiscountCodeId");
			int? currentDiscountCodeId = null;
			if (!string.IsNullOrEmpty(currentDiscountCodeIdStr) && int.TryParse(currentDiscountCodeIdStr, out var codeId))
			{
				currentDiscountCodeId = codeId;
			}

			// Filter out already applied code and get new available codes
			var newAvailableCodes = availableDiscountCodes
				.Where(dc => !currentDiscountCodeId.HasValue || dc.Id != currentDiscountCodeId.Value)
				.ToList();

			return Json(new { 
				success = true,
				cartTotal = cartTotal.Value,
				availableCodes = newAvailableCodes,
				hasNewCodes = newAvailableCodes.Any()
			});
		}

		[HttpPost]
		[IgnoreAntiforgeryToken]
		public async Task<IActionResult> ApplyDiscountCode(string discountCode)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(discountCode))
				{
					return Json(new { success = false, message = "Vui lòng nhập mã giảm giá" });
				}

				// Trim and uppercase the code
				var trimmedCode = discountCode.Trim().ToUpper();
				
				// Get cart items first
				var cartItems = _cartService.GetCart();
				if (!cartItems.Any())
				{
					return Json(new { success = false, message = "Giỏ hàng trống" });
				}

				var cartTotal = cartItems.Sum(i => i.UnitPrice * i.Quantity);

				// Find discount code
				var code = await _context.DiscountCodes
					.FirstOrDefaultAsync(dc => dc.Code.ToUpper() == trimmedCode);

				if (code == null)
				{
					return Json(new { success = false, message = $"Mã giảm giá '{trimmedCode}' không tồn tại" });
				}

				// Check if active
				if (!code.IsActive)
				{
					return Json(new { success = false, message = "Mã giảm giá đã bị vô hiệu hóa" });
				}

				// Check date range
				var now = DateTime.UtcNow;
				if (now < code.StartDate)
				{
					return Json(new { success = false, message = $"Mã giảm giá chưa có hiệu lực (bắt đầu từ {code.StartDate:dd/MM/yyyy})" });
				}
				
				if (now > code.EndDate)
				{
					return Json(new { success = false, message = $"Mã giảm giá đã hết hạn (hết hạn {code.EndDate:dd/MM/yyyy})" });
				}

				// Check usage limit
				if (code.UsageLimit > 0 && code.UsageCount >= code.UsageLimit)
				{
					return Json(new { success = false, message = "Mã giảm giá đã hết lượt sử dụng" });
				}

				// Check minimum order amount
				if (code.MinimumOrderAmount.HasValue && code.MinimumOrderAmount.Value > 0)
				{
					if (cartTotal < code.MinimumOrderAmount.Value)
					{
						return Json(new { 
							success = false, 
							message = $"Đơn hàng tối thiểu {code.MinimumOrderAmount.Value.ToString("N0")} đ để sử dụng mã này. Giá trị đơn hàng hiện tại: {cartTotal.ToString("N0")} đ" 
						});
					}
				}

				// Calculate discount
				decimal discountAmount = 0;
				if (code.DiscountPercent.HasValue && code.DiscountPercent.Value > 0)
				{
					discountAmount = cartTotal * (code.DiscountPercent.Value / 100);
				}
				else if (code.DiscountAmount.HasValue && code.DiscountAmount.Value > 0)
				{
					discountAmount = code.DiscountAmount.Value;
				}

				// Ensure discount doesn't exceed cart total
				discountAmount = Math.Min(discountAmount, cartTotal);
				var finalTotal = Math.Max(0, cartTotal - discountAmount);

				// Store discount code in session
				HttpContext.Session.SetString("AppliedDiscountCode", code.Code);
				HttpContext.Session.SetString("DiscountCodeId", code.Id.ToString());

				return Json(new { 
					success = true, 
					message = $"Áp dụng mã giảm giá thành công! Tiết kiệm {discountAmount.ToString("N0")} đ",
					discountAmount = discountAmount,
					discountPercent = code.DiscountPercent,
					discountFixed = code.DiscountAmount,
					cartTotal = cartTotal,
					finalTotal = finalTotal,
					code = code.Code
				});
			}
			catch (Exception ex)
			{
				return Json(new { 
					success = false, 
					message = $"Có lỗi xảy ra: {ex.Message}" 
				});
			}
		}

		[HttpPost]
		[IgnoreAntiforgeryToken]
		public IActionResult RemoveDiscountCode()
		{
			HttpContext.Session.Remove("AppliedDiscountCode");
			HttpContext.Session.Remove("DiscountCodeId");
			
			var cartItems = _cartService.GetCart();
			var cartTotal = cartItems.Sum(i => i.UnitPrice * i.Quantity);

			return Json(new { 
				success = true, 
				message = "Đã xóa mã giảm giá",
				cartTotal = cartTotal,
				finalTotal = cartTotal
			});
		}

		[Authorize]
		public async Task<IActionResult> Checkout()
		{
			var adminCheck = await CheckAdminAndRedirectAsync();
			if (adminCheck != null) return adminCheck;

			var items = _cartService.GetCart();
			if (!items.Any())
			{
				return RedirectToAction(nameof(Index));
			}

			var viewModel = new List<(CartItem Item, Product? Product)>();
			foreach (var item in items)
			{
				var product = await _productRepository.GetByIdAsync(item.ProductId);
				viewModel.Add((item, product));
			}

			var cartTotal = items.Sum(i => i.UnitPrice * i.Quantity);
			decimal discountAmount = 0;
			string? appliedDiscountCode = null;

			// Check if discount code is applied
			var discountCodeStr = HttpContext.Session.GetString("AppliedDiscountCode");
			if (!string.IsNullOrEmpty(discountCodeStr))
			{
				var discountCodeIdStr = HttpContext.Session.GetString("DiscountCodeId");
				if (!string.IsNullOrEmpty(discountCodeIdStr) && int.TryParse(discountCodeIdStr, out var discountCodeId))
				{
					var discountCode = await _context.DiscountCodes.FindAsync(discountCodeId);
					if (discountCode != null && discountCode.IsActive)
					{
						var now = DateTime.UtcNow;
						if (now >= discountCode.StartDate && now <= discountCode.EndDate)
						{
							if (discountCode.DiscountPercent.HasValue)
							{
								discountAmount = cartTotal * (discountCode.DiscountPercent.Value / 100);
							}
							else if (discountCode.DiscountAmount.HasValue)
							{
								discountAmount = discountCode.DiscountAmount.Value;
							}
							appliedDiscountCode = discountCode.Code;
						}
					}
				}
			}

			var finalTotal = Math.Max(0, cartTotal - discountAmount);

			var checkoutModel = new CheckoutViewModel
			{
				CartItems = viewModel,
				Total = finalTotal
			};

			var user = await _userManager.GetUserAsync(User);
			if (user != null)
			{
				checkoutModel.FullName = user.UserName ?? string.Empty;
				checkoutModel.PhoneNumber = user.PhoneNumber ?? string.Empty;
			}

			ViewBag.CartTotal = cartTotal;
			ViewBag.DiscountAmount = discountAmount;
			ViewBag.AppliedDiscountCode = appliedDiscountCode;

			return View(checkoutModel);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Checkout(CheckoutViewModel model)
		{
			var adminCheck = await CheckAdminAndRedirectAsync();
			if (adminCheck != null) return adminCheck;
			if (!ModelState.IsValid)
			{
				var items = _cartService.GetCart();
				var viewModel = new List<(CartItem Item, Product? Product)>();
				foreach (var item in items)
				{
					var product = await _productRepository.GetByIdAsync(item.ProductId);
					viewModel.Add((item, product));
				}
				model.CartItems = viewModel;
				
				// Recalculate totals with discount
				var invalidCartTotal = items.Sum(i => i.UnitPrice * i.Quantity);
				decimal invalidDiscountAmount = 0;
				string? invalidAppliedDiscountCode = null;

				var invalidDiscountCodeStr = HttpContext.Session.GetString("AppliedDiscountCode");
				if (!string.IsNullOrEmpty(invalidDiscountCodeStr))
				{
					var invalidDiscountCodeIdStr = HttpContext.Session.GetString("DiscountCodeId");
					if (!string.IsNullOrEmpty(invalidDiscountCodeIdStr) && int.TryParse(invalidDiscountCodeIdStr, out var invalidDiscountCodeId))
					{
						var invalidDiscountCode = await _context.DiscountCodes.FindAsync(invalidDiscountCodeId);
						if (invalidDiscountCode != null && invalidDiscountCode.IsActive)
						{
							var now = DateTime.UtcNow;
							if (now >= invalidDiscountCode.StartDate && now <= invalidDiscountCode.EndDate)
							{
								if (invalidDiscountCode.DiscountPercent.HasValue)
								{
									invalidDiscountAmount = invalidCartTotal * (invalidDiscountCode.DiscountPercent.Value / 100);
								}
								else if (invalidDiscountCode.DiscountAmount.HasValue)
								{
									invalidDiscountAmount = invalidDiscountCode.DiscountAmount.Value;
								}
								invalidAppliedDiscountCode = invalidDiscountCode.Code;
							}
						}
					}
				}

				var invalidFinalTotal = Math.Max(0, invalidCartTotal - invalidDiscountAmount);
				model.Total = invalidFinalTotal;
				
				ViewBag.CartTotal = invalidCartTotal;
				ViewBag.DiscountAmount = invalidDiscountAmount;
				ViewBag.AppliedDiscountCode = invalidAppliedDiscountCode;
				
				return View(model);
			}

			var cartItems = _cartService.GetCart();
			if (!cartItems.Any())
			{
				return RedirectToAction(nameof(Index));
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized();

			// Calculate totals with discount
			var cartTotal = cartItems.Sum(i => i.UnitPrice * i.Quantity);
			decimal discountAmount = 0;
			int? discountCodeId = null;

			var discountCodeStr = HttpContext.Session.GetString("AppliedDiscountCode");
			if (!string.IsNullOrEmpty(discountCodeStr))
			{
				var discountCodeIdStr = HttpContext.Session.GetString("DiscountCodeId");
				if (!string.IsNullOrEmpty(discountCodeIdStr) && int.TryParse(discountCodeIdStr, out var codeId))
				{
					var discountCode = await _context.DiscountCodes.FindAsync(codeId);
					if (discountCode != null && discountCode.IsActive)
					{
						var now = DateTime.UtcNow;
						if (now >= discountCode.StartDate && now <= discountCode.EndDate)
						{
							if (discountCode.DiscountPercent.HasValue)
							{
								discountAmount = cartTotal * (discountCode.DiscountPercent.Value / 100);
							}
							else if (discountCode.DiscountAmount.HasValue)
							{
								discountAmount = discountCode.DiscountAmount.Value;
							}
							discountCodeId = discountCode.Id;
							
							// Increment usage count
							discountCode.UsageCount++;
						}
					}
				}
			}

			var finalTotal = Math.Max(0, cartTotal - discountAmount);

			// Create Order
			var order = new Order
			{
				UserId = user.Id,
				OrderDate = DateTime.UtcNow,
				Status = "Pending",
				ShippingAddress = model.ShippingAddress,
				Total = finalTotal,
				PaymentMethod = model.PaymentMethod
			};

			foreach (var cartItem in cartItems)
			{
				var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
				if (product != null && product.Stock >= cartItem.Quantity)
				{
					order.Items.Add(new OrderItem
					{
						ProductId = product.Id,
						Quantity = cartItem.Quantity,
						UnitPrice = cartItem.UnitPrice
					});
					product.Stock -= cartItem.Quantity;
				}
			}

			if (!order.Items.Any())
			{
				ModelState.AddModelError("", "Một số sản phẩm không còn đủ hàng.");
				var viewModel = new List<(CartItem Item, Product? Product)>();
				foreach (var item in cartItems)
				{
					var product = await _productRepository.GetByIdAsync(item.ProductId);
					viewModel.Add((item, product));
				}
				model.CartItems = viewModel;
				model.Total = cartItems.Sum(i => i.UnitPrice * i.Quantity);
				return View(model);
			}

			await _orderRepository.AddAsync(order);
			await _context.SaveChangesAsync();

			// Clear discount code from session after successful order
			HttpContext.Session.Remove("AppliedDiscountCode");
			HttpContext.Session.Remove("DiscountCodeId");

			// Clear cart
			_cartService.Clear();

			// Store order ID in TempData
			TempData["OrderId"] = order.Id;

			// Handle different payment methods
			switch (model.PaymentMethod)
			{
				case Models.PaymentMethod.VietQR:
					// VietQR: Redirect to payment confirmation page with QR code
					return RedirectToAction("Confirm", "Payment", new { orderId = order.Id });

				case Models.PaymentMethod.COD:
					// COD: Order is already created, just redirect to success page
					order.Status = "Confirmed";
					await _context.SaveChangesAsync();
					return RedirectToAction(nameof(OrderCompleted), new { orderId = order.Id });

				case Models.PaymentMethod.BankTransfer:
					// Bank Transfer: Order is pending, redirect to bank transfer instructions
					TempData["BankTransferOrderId"] = order.Id;
					TempData["BankTransferAmount"] = order.Total.ToString("F2");
					return RedirectToAction(nameof(BankTransferInstructions));

				default:
					return RedirectToAction(nameof(OrderCompleted), new { orderId = order.Id });
			}
		}

		public IActionResult BankTransferInstructions()
		{
			var orderId = TempData["BankTransferOrderId"] != null ? Convert.ToInt32(TempData["BankTransferOrderId"]) : (int?)null;
			var amount = TempData["BankTransferAmount"] != null ? 
				(decimal.TryParse(TempData["BankTransferAmount"]?.ToString(), out var parsedAmount) ? parsedAmount : 0) : 0;

			if (orderId == null)
			{
				return RedirectToAction(nameof(Index));
			}

			ViewBag.OrderId = orderId;
			ViewBag.Amount = amount;
			return View();
		}

		public async Task<IActionResult> OrderCompleted(int? orderId)
		{
			if (orderId == null && TempData["OrderId"] != null)
			{
				orderId = Convert.ToInt32(TempData["OrderId"]);
			}
			
			if (orderId.HasValue)
			{
				var order = await _orderRepository.GetByIdAsync(orderId.Value);
				if (order != null)
				{
					// Calculate original total (sum of all items)
					var originalTotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
					var discountAmount = originalTotal - order.Total;
					
					ViewBag.OrderId = order.Id;
					ViewBag.OriginalTotal = originalTotal;
					ViewBag.DiscountAmount = discountAmount;
					ViewBag.OrderDate = order.OrderDate;
					ViewBag.PaymentMethod = order.PaymentMethod.ToString();
					ViewBag.OrderTotal = order.Total;
				}
			}
			else
			{
				ViewBag.OrderId = null;
			}
			
			return View();
		}
	}
}


