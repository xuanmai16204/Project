namespace WebsiteDienNha_DoAnChuyenNganh.DTO.Shipping.GHN
{
	public class GHNCalculateFeeRequest
	{
		public int ServiceId { get; set; }
		public string FromDistrictId { get; set; } = string.Empty;
		public string ToDistrictId { get; set; } = string.Empty;
		public string ToWardCode { get; set; } = string.Empty;
		public decimal Weight { get; set; }
		public decimal Length { get; set; }
		public decimal Width { get; set; }
		public decimal Height { get; set; }
		public decimal InsuranceValue { get; set; }
	}
}


