using System.Collections.Generic;
using System.Threading.Tasks;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.IRepository
{
	public interface ICategoryRepository
	{
		Task<IEnumerable<Category>> GetAllAsync();
		Task<Category?> GetByIdAsync(int id);
		Task<Category> AddAsync(Category category);
		Task UpdateAsync(Category category);
		Task DeleteAsync(int id);
	}
}


