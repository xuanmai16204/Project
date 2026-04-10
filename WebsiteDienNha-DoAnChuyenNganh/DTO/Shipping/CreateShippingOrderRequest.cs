namespace WebsiteDienNha_DoAnChuyenNganh.DTO.Shipping
{
	public class CreateShippingOrderRequest
	{
		public string OrderCode { get; set; } = string.Empty;
		public string RecipientName { get; set; } = string.Empty;
		public string RecipientPhone { get; set; } = string.Empty;
		public string RecipientAddress { get; set; } = string.Empty;
		public string Province { get; set; } = string.Empty;
		public string District { get; set; } = string.Empty;
		public string Ward { get; set; } = string.Empty;
		public decimal Weight { get; set; }
		public decimal CodAmount { get; set; }
		public string ServiceId { get; set; } = string.Empty;
	}
}


