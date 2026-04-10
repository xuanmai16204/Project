using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.RoleAdmin)]
	public class HomeController : Controller
	{
		private readonly ApplicationDbContext _context;

		public HomeController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var today = DateTime.Today;
			var thisMonth = new DateTime(today.Year, today.Month, 1);
			var lastMonth = thisMonth.AddMonths(-1);

			// Thống kê tổng quan
			var totalOrders = await _context.Orders.CountAsync();
			var totalRevenue = await _context.Orders.SumAsync(o => o.Total);
			var totalProducts = await _context.Products.CountAsync();
			var totalCustomers = await _context.Users.CountAsync();

			// Thống kê hôm nay
			var todayOrders = await _context.Orders
				.Where(o => o.OrderDate.Date == today)
				.CountAsync();
			var todayRevenue = await _context.Orders
				.Where(o => o.OrderDate.Date == today)
				.SumAsync(o => (decimal?)o.Total) ?? 0;

			// Thống kê tháng này
			var monthOrders = await _context.Orders
				.Where(o => o.OrderDate >= thisMonth)
				.CountAsync();
			var monthRevenue = await _context.Orders
				.Where(o => o.OrderDate >= thisMonth)
				.SumAsync(o => (decimal?)o.Total) ?? 0;

			// Đơn hàng gần đây
			var recentOrders = await _context.Orders
				.Include(o => o.User)
				.OrderByDescending(o => o.OrderDate)
				.Take(10)
				.ToListAsync();

			// Thống kê theo trạng thái
			var pendingOrders = await _context.Orders.CountAsync(o => o.Status == "Pending");
			var confirmedOrders = await _context.Orders.CountAsync(o => o.Status == "Confirmed");
			var paidOrders = await _context.Orders.CountAsync(o => o.Status == "Paid");

			ViewBag.TotalOrders = totalOrders;
			ViewBag.TotalRevenue = totalRevenue;
			ViewBag.TotalProducts = totalProducts;
			ViewBag.TotalCustomers = totalCustomers;
			ViewBag.TodayOrders = todayOrders;
			ViewBag.TodayRevenue = todayRevenue;
			ViewBag.MonthOrders = monthOrders;
			ViewBag.MonthRevenue = monthRevenue;
			ViewBag.RecentOrders = recentOrders;
			ViewBag.PendingOrders = pendingOrders;
			ViewBag.ConfirmedOrders = confirmedOrders;
			ViewBag.PaidOrders = paidOrders;

			return View();
		}
	}
}

