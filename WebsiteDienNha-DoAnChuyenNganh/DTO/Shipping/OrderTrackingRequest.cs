namespace WebsiteDienNha_DoAnChuyenNganh.DTO.Shipping
{
	public class OrderTrackingRequest
	{
		public string TrackingNumber { get; set; } = string.Empty;
		public string Carrier { get; set; } = "GHN";
	}
}


