using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Controllers
{
	[Authorize]
	public class OrderController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		private async Task<IActionResult> CheckAdminAndRedirectAsync()
		{
			if (User.Identity?.IsAuthenticated == true)
			{
				var user = await _userManager.GetUserAsync(User);
				if (user != null && await _userManager.IsInRoleAsync(user, SD.RoleAdmin))
				{
					TempData["ErrorMessage"] = "Admin không thể xem đơn hàng của khách hàng. Vui lòng sử dụng trang quản lý đơn hàng.";
					return RedirectToAction("Index", "Order", new { area = "Admin" });
				}
			}
			return null!;
		}

		public async Task<IActionResult> Details(int? id)
		{
			var adminCheck = await CheckAdminAndRedirectAsync();
			if (adminCheck != null) return adminCheck;

			if (id == null)
			{
				return NotFound();
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized();

			var order = await _context.Orders
				.Include(o => o.Items)
				.ThenInclude(i => i.Product)
				.FirstOrDefaultAsync(m => m.Id == id && m.UserId == user.Id);

			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}

		public async Task<IActionResult> History()
		{
			var adminCheck = await CheckAdminAndRedirectAsync();
			if (adminCheck != null) return adminCheck;

			var user = await _userManager.GetUserAsync(User);
			if (user == null) return Unauthorized();

			var orders = await _context.Orders
				.Where(o => o.UserId == user.Id)
				.OrderByDescending(o => o.OrderDate)
				.ToListAsync();

			return View(orders);
		}
	}
}

