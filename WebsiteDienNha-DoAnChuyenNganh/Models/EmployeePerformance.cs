using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class EmployeePerformance
	{
		public int Id { get; set; }

		public int EmployeeId { get; set; }
		public Employee? Employee { get; set; }

		[Range(1, 12)]
		public int Month { get; set; }

		public int Year { get; set; }

		public int OrdersHandled { get; set; }

		public decimal RevenueGenerated { get; set; }

		public int TicketsResolved { get; set; }

		[Range(0, 100)]
		public int Score { get; set; }

		[MaxLength(500)]
		public string? Notes { get; set; }
	}
}


