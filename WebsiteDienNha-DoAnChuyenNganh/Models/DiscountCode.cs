using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class DiscountCode
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string Code { get; set; } = string.Empty;

		[MaxLength(250)]
		public string? Description { get; set; }

		public decimal? DiscountPercent { get; set; }

		public decimal? DiscountAmount { get; set; }

		public DateTime StartDate { get; set; } = DateTime.UtcNow;

		public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);

		public int UsageLimit { get; set; } = 0;

		public int UsageCount { get; set; } = 0;

		public decimal? MinimumOrderAmount { get; set; }

		public bool IsActive { get; set; } = true;

		[MaxLength(100)]
		public string? CreatedBy { get; set; }
	}
}


