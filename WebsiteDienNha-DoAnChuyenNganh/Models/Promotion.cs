using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class Promotion
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(150)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(500)]
		public string? Description { get; set; }

		[MaxLength(500)]
		public string? BannerUrl { get; set; }

		public DateTime StartDate { get; set; } = DateTime.UtcNow;

		public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);

		public decimal? DiscountPercent { get; set; }

		public decimal? DiscountAmount { get; set; }

		public decimal? MinOrderTotal { get; set; }

		public decimal? MaxDiscount { get; set; }

		public bool IsActive { get; set; } = true;

		public ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();
		public ICollection<PromotionUsage> PromotionUsages { get; set; } = new List<PromotionUsage>();
	}
}


