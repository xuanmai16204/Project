using Microsoft.AspNetCore.Mvc;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.IRepository;
using WebsiteDienNha_DoAnChuyenNganh.Models;
using WebsiteDienNha_DoAnChuyenNganh.Services;

namespace WebsiteDienNha_DoAnChuyenNganh.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class PaymentController : Controller
	{
		private readonly IOrderRepository _orderRepository;
		private readonly ApplicationDbContext _context;
		private readonly VietQRService _vietQRService;

		public PaymentController(
			IOrderRepository orderRepository,
			ApplicationDbContext context,
			VietQRService vietQRService)
		{
			_orderRepository = orderRepository;
			_context = context;
			_vietQRService = vietQRService;
		}

		[HttpGet]
		public async Task<IActionResult> Confirm(int orderId)
		{
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
				return RedirectToAction("OrderCompleted", "ShoppingCart", new { orderId = orderId });
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

			return Json(new { success = true, message = "Xác nhận thanh toán thành công!", redirectUrl = Url.Action("OrderCompleted", "ShoppingCart", new { orderId = request.OrderId }) });
		}
	}

	public class ConfirmPaymentRequest
	{
		public int OrderId { get; set; }
	}
}

