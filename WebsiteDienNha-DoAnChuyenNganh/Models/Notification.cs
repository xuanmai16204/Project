using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class Notification
	{
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; } = string.Empty;
		public ApplicationUser? User { get; set; }

		[MaxLength(150)]
		public string Title { get; set; } = string.Empty;

		[MaxLength(1000)]
		public string Message { get; set; } = string.Empty;

		[MaxLength(50)]
		public string Type { get; set; } = "info";

		public bool IsRead { get; set; } = false;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[MaxLength(500)]
		public string? Link { get; set; }
	}
}


