using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.RoleAdmin)]
	public class DiscountCodeController : Controller
	{
		private readonly ApplicationDbContext _context;

		public DiscountCodeController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var discountCodes = await _context.DiscountCodes
				.OrderByDescending(dc => dc.CreatedBy)
				.ThenByDescending(dc => dc.StartDate)
				.ToListAsync();
			return View(discountCodes);
		}

		public IActionResult Create()
		{
			return View(new DiscountCode
			{
				StartDate = DateTime.UtcNow,
				EndDate = DateTime.UtcNow.AddMonths(1),
				IsActive = true
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DiscountCode model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Validate that either DiscountPercent or DiscountAmount is provided
			if (!model.DiscountPercent.HasValue && !model.DiscountAmount.HasValue)
			{
				ModelState.AddModelError("", "Vui lòng nhập phần trăm giảm giá hoặc số tiền giảm giá.");
				return View(model);
			}

			// Validate that not both are provided
			if (model.DiscountPercent.HasValue && model.DiscountAmount.HasValue)
			{
				ModelState.AddModelError("", "Chỉ có thể chọn một trong hai: phần trăm giảm giá hoặc số tiền giảm giá.");
				return View(model);
			}

			// Validate dates
			if (model.EndDate <= model.StartDate)
			{
				ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu.");
				return View(model);
			}

			// Check if code already exists
			var existingCode = await _context.DiscountCodes
				.FirstOrDefaultAsync(dc => dc.Code.ToUpper() == model.Code.ToUpper());
			if (existingCode != null)
			{
				ModelState.AddModelError("Code", "Mã giảm giá đã tồn tại.");
				return View(model);
			}

			// Set created by
			model.CreatedBy = User.Identity?.Name;
			model.Code = model.Code.ToUpper().Trim();

			_context.DiscountCodes.Add(model);
			await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = "Tạo mã giảm giá thành công!";
			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var discountCode = await _context.DiscountCodes.FindAsync(id);
			if (discountCode == null)
			{
				return NotFound();
			}

			return View(discountCode);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, DiscountCode model)
		{
			if (id != model.Id)
			{
				return NotFound();
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Validate that either DiscountPercent or DiscountAmount is provided
			if (!model.DiscountPercent.HasValue && !model.DiscountAmount.HasValue)
			{
				ModelState.AddModelError("", "Vui lòng nhập phần trăm giảm giá hoặc số tiền giảm giá.");
				return View(model);
			}

			// Validate that not both are provided
			if (model.DiscountPercent.HasValue && model.DiscountAmount.HasValue)
			{
				ModelState.AddModelError("", "Chỉ có thể chọn một trong hai: phần trăm giảm giá hoặc số tiền giảm giá.");
				return View(model);
			}

			// Validate dates
			if (model.EndDate <= model.StartDate)
			{
				ModelState.AddModelError("EndDate", "Ngày kết thúc phải sau ngày bắt đầu.");
				return View(model);
			}

			// Check if code already exists (excluding current record)
			var existingCode = await _context.DiscountCodes
				.FirstOrDefaultAsync(dc => dc.Code.ToUpper() == model.Code.ToUpper() && dc.Id != id);
			if (existingCode != null)
			{
				ModelState.AddModelError("Code", "Mã giảm giá đã tồn tại.");
				return View(model);
			}

			try
			{
				model.Code = model.Code.ToUpper().Trim();
				_context.Update(model);
				await _context.SaveChangesAsync();

				TempData["SuccessMessage"] = "Cập nhật mã giảm giá thành công!";
				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await DiscountCodeExists(model.Id))
				{
					return NotFound();
				}
				throw;
			}
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var discountCode = await _context.DiscountCodes.FindAsync(id);
			if (discountCode == null)
			{
				return NotFound();
			}

			return View(discountCode);
		}

		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var discountCode = await _context.DiscountCodes.FindAsync(id);
			if (discountCode == null)
			{
				return NotFound();
			}

			return View(discountCode);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var discountCode = await _context.DiscountCodes.FindAsync(id);
			if (discountCode != null)
			{
				_context.DiscountCodes.Remove(discountCode);
				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "Xóa mã giảm giá thành công!";
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ToggleActive(int id)
		{
			var discountCode = await _context.DiscountCodes.FindAsync(id);
			if (discountCode == null)
			{
				return NotFound();
			}

			discountCode.IsActive = !discountCode.IsActive;
			await _context.SaveChangesAsync();

			TempData["SuccessMessage"] = discountCode.IsActive 
				? "Kích hoạt mã giảm giá thành công!" 
				: "Vô hiệu hóa mã giảm giá thành công!";
			
			return RedirectToAction(nameof(Index));
		}

		private async Task<bool> DiscountCodeExists(int id)
		{
			return await _context.DiscountCodes.AnyAsync(e => e.Id == id);
		}
	}
}

