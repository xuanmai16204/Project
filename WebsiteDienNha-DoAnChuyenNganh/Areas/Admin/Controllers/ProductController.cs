using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebsiteDienNha_DoAnChuyenNganh.IRepository;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.RoleAdmin)]
	public class ProductController : Controller
	{
		private readonly IProductRepository _productRepo;
		private readonly ICategoryRepository _categoryRepo;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ProductController(IProductRepository productRepo, ICategoryRepository categoryRepo, IWebHostEnvironment webHostEnvironment)
		{
			_productRepo = productRepo;
			_categoryRepo = categoryRepo;
			_webHostEnvironment = webHostEnvironment;
		}

		public async Task<IActionResult> Index()
		{
			var products = await _productRepo.GetAllAsync();
			return View(products);
		}

		private async Task PopulateCategoriesAsync(object? selected = null)
		{
			var cats = await _categoryRepo.GetAllAsync();
			ViewBag.CategoryId = new SelectList(cats, "Id", "Name", selected);
		}

		public async Task<IActionResult> Create()
		{
			await PopulateCategoriesAsync();
			return View(new Product());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Product model, IFormFile? imageFile)
		{
			if (!ModelState.IsValid)
			{
				await PopulateCategoriesAsync(model.CategoryId);
				return View(model);
			}

			// Xử lý upload ảnh
			if (imageFile != null && imageFile.Length > 0)
			{
				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await imageFile.CopyToAsync(fileStream);
				}

				model.ImageUrl = "/images/products/" + uniqueFileName;
			}

			await _productRepo.AddAsync(model);
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Edit(int id)
		{
			var entity = await _productRepo.GetByIdAsync(id);
			if (entity == null) return NotFound();
			await PopulateCategoriesAsync(entity.CategoryId);
			return View(entity);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Product model, IFormFile? imageFile)
		{
			if (!ModelState.IsValid)
			{
				await PopulateCategoriesAsync(model.CategoryId);
				return View(model);
			}

			// Lấy sản phẩm hiện tại để giữ ImageUrl nếu không upload ảnh mới
			var existingProduct = await _productRepo.GetByIdAsync(model.Id);
			if (existingProduct == null)
			{
				return NotFound();
			}

			// Xử lý upload ảnh mới
			if (imageFile != null && imageFile.Length > 0)
			{
				var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				// Xóa ảnh cũ nếu có
				if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
				{
					var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.ImageUrl.TrimStart('/'));
					if (System.IO.File.Exists(oldImagePath))
					{
						System.IO.File.Delete(oldImagePath);
					}
				}

				var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await imageFile.CopyToAsync(fileStream);
				}

				model.ImageUrl = "/images/products/" + uniqueFileName;
			}
			else
			{
				// Giữ lại ImageUrl cũ nếu không upload ảnh mới
				model.ImageUrl = existingProduct.ImageUrl;
			}

			await _productRepo.UpdateAsync(model);
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Details(int id)
		{
			var entity = await _productRepo.GetByIdAsync(id);
			if (entity == null) return NotFound();
			return View(entity);
		}

		public async Task<IActionResult> Delete(int id)
		{
			var entity = await _productRepo.GetByIdAsync(id);
			if (entity == null) return NotFound();
			return View(entity);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			await _productRepo.DeleteAsync(id);
			return RedirectToAction(nameof(Index));
		}
	}
}


