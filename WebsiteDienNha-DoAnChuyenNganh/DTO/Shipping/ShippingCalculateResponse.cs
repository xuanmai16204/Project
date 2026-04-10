namespace WebsiteDienNha_DoAnChuyenNganh.DTO.Shipping
{
	public class ShippingCalculateResponse
	{
		public decimal Fee { get; set; }
		public decimal InsuranceFee { get; set; }
		public decimal EstimatedDeliveryDays { get; set; }
		public string ServiceName { get; set; } = string.Empty;
	}
}


