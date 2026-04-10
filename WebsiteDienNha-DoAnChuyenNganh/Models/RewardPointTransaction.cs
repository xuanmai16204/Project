using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class RewardPointTransaction
	{
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; } = string.Empty;
		public ApplicationUser? User { get; set; }

		public int Points { get; set; }

		[MaxLength(200)]
		public string Reason { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public int? OrderId { get; set; }
		public Order? Order { get; set; }
	}
}


