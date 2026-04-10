using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.IRepository;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly ApplicationDbContext _db;

		public ProductRepository(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<Product>> GetAllAsync()
		{
			return await _db.Products.Include(p => p.Category).AsNoTracking().ToListAsync();
		}

		public async Task<Product?> GetByIdAsync(int id)
		{
			return await _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<Product> AddAsync(Product product)
		{
			_db.Products.Add(product);
			await _db.SaveChangesAsync();
			return product;
		}

		public async Task UpdateAsync(Product product)
		{
			// Kiểm tra xem entity có đang được track không
			var trackedEntity = _db.Products.Local.FirstOrDefault(p => p.Id == product.Id);
			if (trackedEntity != null)
			{
				// Detach entity đang được track
				_db.Entry(trackedEntity).State = EntityState.Detached;
			}

			// Load entity từ DB
			var entity = await _db.Products.FindAsync(product.Id);
			if (entity == null)
			{
				throw new InvalidOperationException($"Product with Id {product.Id} not found.");
			}

			// Update properties từ model vào entity
			entity.Name = product.Name;
			entity.Slug = product.Slug;
			entity.Description = product.Description;
			entity.Price = product.Price;
			entity.PromotionPrice = product.PromotionPrice;
			entity.Stock = product.Stock;
			entity.IsActive = product.IsActive;
			entity.ImageUrl = product.ImageUrl;
			entity.CategoryId = product.CategoryId;
			entity.UpdatedAt = DateTime.UtcNow;

			await _db.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var entity = await _db.Products.FindAsync(id);
			if (entity != null)
			{
				_db.Products.Remove(entity);
				await _db.SaveChangesAsync();
			}
		}
	}
}


