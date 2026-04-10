using System;

namespace WebsiteDienNha_DoAnChuyenNganh.DTO
{
	public class OrderStatisticDTO
	{
		public DateTime Date { get; set; }
		public int Orders { get; set; }
		public int Customers { get; set; }
		public decimal Revenue { get; set; }
	}
}

