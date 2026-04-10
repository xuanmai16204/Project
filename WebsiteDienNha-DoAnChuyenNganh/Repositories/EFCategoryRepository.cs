using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.IRepository;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Repositories
{
	public class EFCategoryRepository : ICategoryRepository
	{
		private readonly ApplicationDbContext _db;

		public EFCategoryRepository(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<Category>> GetAllAsync()
		{
			return await _db.Categories.AsNoTracking().ToListAsync();
		}

		public async Task<Category?> GetByIdAsync(int id)
		{
			return await _db.Categories.FindAsync(id);
		}

		public async Task<Category> AddAsync(Category category)
		{
			_db.Categories.Add(category);
			await _db.SaveChangesAsync();
			return category;
		}

		public async Task UpdateAsync(Category category)
		{
			_db.Categories.Update(category);
			await _db.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var entity = await _db.Categories.FindAsync(id);
			if (entity != null)
			{
				_db.Categories.Remove(entity);
				await _db.SaveChangesAsync();
			}
		}
	}
}


