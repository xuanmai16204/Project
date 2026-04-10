namespace WebsiteDienNha_DoAnChuyenNganh.DTO
{
	public class EmployeePerformanceDTO
	{
		public string EmployeeCode { get; set; } = string.Empty;
		public string EmployeeName { get; set; } = string.Empty;
		public int Month { get; set; }
		public int Year { get; set; }
		public int OrdersHandled { get; set; }
		public decimal RevenueGenerated { get; set; }
		public int TicketsResolved { get; set; }
		public int Score { get; set; }
	}
}


