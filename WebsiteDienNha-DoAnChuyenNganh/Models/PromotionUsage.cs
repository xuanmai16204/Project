using System;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class PromotionUsage
	{
		public int Id { get; set; }

		public int PromotionId { get; set; }
		public Promotion? Promotion { get; set; }

		public int? OrderId { get; set; }
		public Order? Order { get; set; }

		public string? UserId { get; set; }
		public ApplicationUser? User { get; set; }

		public DateTime UsedAt { get; set; } = DateTime.UtcNow;

		public decimal DiscountApplied { get; set; }
	}
}


