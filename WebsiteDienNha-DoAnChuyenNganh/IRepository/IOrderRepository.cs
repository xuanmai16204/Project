using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.IRepository
{
	public interface IOrderRepository
	{
		Task<IEnumerable<Order>> GetAllAsync();
		Task<Order?> GetByIdAsync(int id);
		Task<Order> AddAsync(Order order);
		Task UpdateAsync(Order order);
	}
}


