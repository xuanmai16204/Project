using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteDienNha_DoAnChuyenNganh.IRepository;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.RoleAdmin)]
	public class CategoryController : Controller
	{
		private readonly ICategoryRepository _repo;

		public CategoryController(ICategoryRepository repo)
		{
			_repo = repo;
		}

		public async Task<IActionResult> Index()
		{
			var categories = await _repo.GetAllAsync();
			return View(categories);
		}

		public IActionResult Create()
		{
			return View(new Category());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Category model)
		{
			if (!ModelState.IsValid) return View(model);
			await _repo.AddAsync(model);
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Edit(int id)
		{
			var entity = await _repo.GetByIdAsync(id);
			if (entity == null) return NotFound();
			return View(entity);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Category model)
		{
			if (!ModelState.IsValid) return View(model);
			await _repo.UpdateAsync(model);
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Details(int id)
		{
			var entity = await _repo.GetByIdAsync(id);
			if (entity == null) return NotFound();
			return View(entity);
		}

		public async Task<IActionResult> Delete(int id)
		{
			var entity = await _repo.GetByIdAsync(id);
			if (entity == null) return NotFound();
			return View(entity);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			await _repo.DeleteAsync(id);
			return RedirectToAction(nameof(Index));
		}
	}
}


