namespace WebsiteDienNha_DoAnChuyenNganh.DTO.Shipping.GHN
{
	public class GHNCalculateFeeResponse
	{
		public decimal TotalFee { get; set; }
		public decimal InsuranceFee { get; set; }
		public decimal CustomFee { get; set; }
		public decimal EstimatedDeliveryDays { get; set; }
	}
}


