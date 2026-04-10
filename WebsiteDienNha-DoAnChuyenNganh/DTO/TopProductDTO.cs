namespace WebsiteDienNha_DoAnChuyenNganh.DTO
{
	public class TopProductDTO
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public string CategoryName { get; set; } = string.Empty;
		public int QuantitySold { get; set; }
		public decimal Revenue { get; set; }
	}
}


