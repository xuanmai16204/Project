using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.Services;

namespace WebsiteDienNha_DoAnChuyenNganh.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class ComparisonController : Controller
	{
		private readonly ComparisonService _comparisonService;
		private readonly ApplicationDbContext _context;

		public ComparisonController(ComparisonService comparisonService, ApplicationDbContext context)
		{
			_comparisonService = comparisonService;
			_context = context;
		}

		// GET: Customer/Comparison
		public async Task<IActionResult> Index()
		{
			var productIds = _comparisonService.GetComparisonList();
			var products = await _context.Products
				.Include(p => p.Category)
				.Where(p => productIds.Contains(p.Id) && p.IsActive)
				.ToListAsync();

			// Sắp xếp theo thứ tự trong danh sách so sánh
			products = products.OrderBy(p => productIds.IndexOf(p.Id)).ToList();

			ViewBag.ComparisonCount = products.Count;
			return View(products);
		}

		// POST: Customer/Comparison/Add
		[HttpPost]
		public async Task<IActionResult> Add(int productId)
		{
			// Kiểm tra sản phẩm có tồn tại không
			var product = await _context.Products
				.Include(p => p.Category)
				.FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);
			
			if (product == null)
			{
				return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
			}

			// Kiểm tra nếu sản phẩm đã có trong danh sách
			var isInList = _comparisonService.IsInComparison(productId);
			if (isInList)
			{
				return Json(new { success = false, message = "Sản phẩm đã có trong danh sách so sánh" });
			}

			// Kiểm tra giới hạn số lượng
			var canAddMore = _comparisonService.CanAddMore();
			if (!canAddMore)
			{
				return Json(new { success = false, message = "Bạn chỉ có thể so sánh tối đa 4 sản phẩm" });
			}

			// Kiểm tra category - chỉ cho phép so sánh sản phẩm cùng loại
			var currentComparisonList = _comparisonService.GetComparisonList();
			if (currentComparisonList.Any())
			{
				// Lấy category của sản phẩm đầu tiên trong danh sách so sánh
				var firstProduct = await _context.Products
					.Include(p => p.Category)
					.FirstOrDefaultAsync(p => p.Id == currentComparisonList.First() && p.IsActive);
				
				if (firstProduct != null)
				{
					// Kiểm tra xem sản phẩm mới có cùng category không
					if (product.CategoryId != firstProduct.CategoryId)
					{
						var categoryName = firstProduct.Category?.Name ?? "chưa phân loại";
						var newCategoryName = product.Category?.Name ?? "chưa phân loại";
						return Json(new { 
							success = false, 
							message = $"Chỉ có thể so sánh các sản phẩm cùng loại. Sản phẩm trong danh sách so sánh thuộc danh mục '{categoryName}', còn sản phẩm bạn muốn thêm thuộc danh mục '{newCategoryName}'. Vui lòng xóa các sản phẩm khác loại trước khi thêm sản phẩm mới." 
						});
					}
				}
			}

			// Thêm sản phẩm vào danh sách so sánh
			var added = _comparisonService.AddProduct(productId);
			
			if (added)
			{
				var count = _comparisonService.GetCount();
				return Json(new { success = true, message = "Đã thêm vào danh sách so sánh", count = count });
			}

			return Json(new { success = false, message = "Không thể thêm sản phẩm vào danh sách so sánh" });
		}

		// POST: Customer/Comparison/Remove
		[HttpPost]
		public IActionResult Remove(int productId)
		{
			var removed = _comparisonService.RemoveProduct(productId);
			
			if (removed)
			{
				var count = _comparisonService.GetCount();
				return Json(new { success = true, message = "Đã xóa khỏi danh sách so sánh", count = count });
			}

			return Json(new { success = false, message = "Không tìm thấy sản phẩm trong danh sách" });
		}

		// POST: Customer/Comparison/Clear
		[HttpPost]
		public IActionResult Clear()
		{
			_comparisonService.Clear();
			return Json(new { success = true, message = "Đã xóa tất cả sản phẩm khỏi danh sách so sánh" });
		}

		// GET: Customer/Comparison/GetCount
		[HttpGet]
		public IActionResult GetCount()
		{
			var count = _comparisonService.GetCount();
			return Json(new { count = count });
		}
	}
}

