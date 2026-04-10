namespace WebsiteDienNha_DoAnChuyenNganh.DTO.Shipping
{
	public class ShippingCalculateRequest
	{
		public string PickupProvince { get; set; } = string.Empty;
		public string PickupDistrict { get; set; } = string.Empty;
		public string PickupWard { get; set; } = string.Empty;
		public string DeliveryProvince { get; set; } = string.Empty;
		public string DeliveryDistrict { get; set; } = string.Empty;
		public string DeliveryWard { get; set; } = string.Empty;
		public decimal Weight { get; set; }
		public decimal Length { get; set; }
		public decimal Width { get; set; }
		public decimal Height { get; set; }
		public decimal DeclaredValue { get; set; }
	}
}


