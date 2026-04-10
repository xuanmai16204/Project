using WebsiteDienNha_DoAnChuyenNganh.Extensions;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Services
{
	public class CartService
	{
		private const string CartSessionKey = "CART_SESSION";
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CartService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		private ISession Session => _httpContextAccessor.HttpContext!.Session;

		public List<CartItem> GetCart()
		{
			return Session.GetObject<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
		}

		public void SaveCart(List<CartItem> items)
		{
			Session.SetObject(CartSessionKey, items);
		}

		public void Clear()
		{
			Session.Remove(CartSessionKey);
		}
	}
}


