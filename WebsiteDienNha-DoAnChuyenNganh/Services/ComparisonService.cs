using WebsiteDienNha_DoAnChuyenNganh.Extensions;

namespace WebsiteDienNha_DoAnChuyenNganh.Services
{
	public class ComparisonService
	{
		private const string ComparisonSessionKey = "COMPARISON_SESSION";
		private readonly IHttpContextAccessor _httpContextAccessor;
		private const int MaxComparisonItems = 4; // Giới hạn tối đa 4 sản phẩm để so sánh

		public ComparisonService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		private ISession Session => _httpContextAccessor.HttpContext!.Session;

		public List<int> GetComparisonList()
		{
			return Session.GetObject<List<int>>(ComparisonSessionKey) ?? new List<int>();
		}

		public void SaveComparisonList(List<int> productIds)
		{
			Session.SetObject(ComparisonSessionKey, productIds);
		}

		public bool AddProduct(int productId)
		{
			var list = GetComparisonList();
			
			// Kiểm tra nếu sản phẩm đã có trong danh sách
			if (list.Contains(productId))
			{
				return false; // Đã tồn tại
			}

			// Kiểm tra giới hạn số lượng
			if (list.Count >= MaxComparisonItems)
			{
				return false; // Đã đạt giới hạn
			}

			list.Add(productId);
			SaveComparisonList(list);
			return true;
		}

		public bool RemoveProduct(int productId)
		{
			var list = GetComparisonList();
			if (list.Remove(productId))
			{
				SaveComparisonList(list);
				return true;
			}
			return false;
		}

		public void Clear()
		{
			Session.Remove(ComparisonSessionKey);
		}

		public int GetCount()
		{
			return GetComparisonList().Count;
		}

		public bool IsInComparison(int productId)
		{
			return GetComparisonList().Contains(productId);
		}

		public bool CanAddMore()
		{
			return GetComparisonList().Count < MaxComparisonItems;
		}
	}
}

