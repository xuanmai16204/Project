using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.IRepository;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Repositories
{
	public class OrderRepository : IOrderRepository
	{
		private readonly ApplicationDbContext _db;

		public OrderRepository(ApplicationDbContext db)
		{
			_db = db;
		}

		public async Task<IEnumerable<Order>> GetAllAsync()
		{
			return await _db.Orders.Include(o => o.Items).ThenInclude(i => i.Product).AsNoTracking().ToListAsync();
		}

		public async Task<Order?> GetByIdAsync(int id)
		{
			return await _db.Orders.Include(o => o.Items).ThenInclude(i => i.Product).FirstOrDefaultAsync(o => o.Id == id);
		}

		public async Task<Order> AddAsync(Order order)
		{
			_db.Orders.Add(order);
			await _db.SaveChangesAsync();
			return order;
		}

		public async Task UpdateAsync(Order order)
		{
			_db.Orders.Update(order);
			await _db.SaveChangesAsync();
		}
	}
}


