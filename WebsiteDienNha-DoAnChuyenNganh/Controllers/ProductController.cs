using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Controllers
{
	public class ProductController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ProductController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index(int? categoryId)
		{
			var query = _context.Products
				.Include(p => p.Category)
				.Include(p => p.PromotionProducts)
					.ThenInclude(pp => pp.Promotion)
				.Where(p => p.IsActive);

			if (categoryId.HasValue)
			{
				query = query.Where(p => p.CategoryId == categoryId.Value);
			}

			var products = await query.ToListAsync();
			ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
			ViewBag.SelectedCategoryId = categoryId;

			return View(products);
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var product = await _context.Products
				.Include(p => p.Category)
				.Include(p => p.PromotionProducts)
					.ThenInclude(pp => pp.Promotion)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (product == null || !product.IsActive)
			{
				return NotFound();
			}

			return View(product);
		}

		[HttpGet]
		public async Task<IActionResult> SearchSuggestions(string query)
		{
			if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
			{
				return Json(new List<object>());
			}

			var products = await _context.Products
				.Where(p => p.IsActive && (p.Name.Contains(query) || p.Description.Contains(query)))
				.Select(p => new { id = p.Id, name = p.Name })
				.Take(10)
				.ToListAsync();

			return Json(products);
		}
	}
}

