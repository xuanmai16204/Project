using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class GameResult
	{
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; } = string.Empty;
		public ApplicationUser? User { get; set; }

		[MaxLength(100)]
		public string GameName { get; set; } = string.Empty;

		public int Score { get; set; }

		public int RewardPoints { get; set; }

		public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
	}
}


