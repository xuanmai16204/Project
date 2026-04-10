using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Services
{
	public class LoyaltyService
	{
		public int CalculatePointsFromOrder(Order order)
		{
			return (int)decimal.Floor(order.Total);
		}
	}
}


