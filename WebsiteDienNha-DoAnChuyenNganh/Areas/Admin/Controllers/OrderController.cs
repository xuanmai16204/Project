using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.RoleAdmin)]
	public class OrderController : Controller
	{
		private readonly ApplicationDbContext _context;

		public OrderController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(string? status = null)
		{
			var query = _context.Orders
				.Include(o => o.User)
				.Include(o => o.Items)
				.AsQueryable();

			if (!string.IsNullOrEmpty(status))
			{
				query = query.Where(o => o.Status == status);
			}

			var orders = await query
				.OrderByDescending(o => o.OrderDate)
				.ToListAsync();

			ViewBag.StatusFilter = status;
			ViewBag.TotalOrders = await _context.Orders.CountAsync();
			ViewBag.PendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");
			ViewBag.ConfirmedOrders = await _context.Orders.CountAsync(o => o.Status == "Confirmed");
			ViewBag.PaidOrders = await _context.Orders.CountAsync(o => o.Status == "Paid");

			return View(orders);
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var order = await _context.Orders
				.Include(o => o.User)
				.Include(o => o.Items)
				.ThenInclude(i => i.Product)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateStatus(int id, string status)
		{
			var order = await _context.Orders.FindAsync(id);
			if (order == null)
			{
				return NotFound();
			}

			order.Status = status;
			await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";
			return RedirectToAction(nameof(Details), new { id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateOrder(int id, string status, string? shippingAddress)
		{
			var order = await _context.Orders
				.Include(o => o.Items)
				.ThenInclude(i => i.Product)
				.FirstOrDefaultAsync(o => o.Id == id);
			
			if (order == null)
			{
				return NotFound();
			}

			// Validate status
			var validStatuses = new[] { "Pending", "Confirmed", "Paid", "Shipping", "Delivered", "Cancelled", "Failed" };
			if (!validStatuses.Contains(status))
			{
				TempData["ErrorMessage"] = "Trạng thái không hợp lệ!";
				return RedirectToAction(nameof(Details), new { id });
			}

			// Update status
			order.Status = status;

			// Update shipping address if provided
			if (!string.IsNullOrWhiteSpace(shippingAddress))
			{
				order.ShippingAddress = shippingAddress;
			}

			await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = "Cập nhật đơn hàng thành công!";
			return RedirectToAction(nameof(Details), new { id });
		}

		public async Task<IActionResult> PrintInvoice(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var order = await _context.Orders
				.Include(o => o.User)
				.Include(o => o.Items)
				.ThenInclude(i => i.Product)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}
	}
}


