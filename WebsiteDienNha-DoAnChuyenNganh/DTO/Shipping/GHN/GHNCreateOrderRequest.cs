using System.Collections.Generic;

namespace WebsiteDienNha_DoAnChuyenNganh.DTO.Shipping.GHN
{
	public class GHNCreateOrderRequest
	{
		public string ClientOrderCode { get; set; } = string.Empty;
		public string ToName { get; set; } = string.Empty;
		public string ToPhone { get; set; } = string.Empty;
		public string ToAddress { get; set; } = string.Empty;
		public string ToWardCode { get; set; } = string.Empty;
		public string ToDistrictId { get; set; } = string.Empty;
		public int ServiceId { get; set; }
		public decimal Weight { get; set; }
		public decimal Length { get; set; }
		public decimal Width { get; set; }
		public decimal Height { get; set; }
		public decimal CodAmount { get; set; }
		public List<GHNItem> Items { get; set; } = new();
	}

	public class GHNItem
	{
		public string Name { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}
}


