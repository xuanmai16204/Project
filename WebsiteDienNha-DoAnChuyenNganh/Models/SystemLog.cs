using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class SystemLog
	{
		public int Id { get; set; }

		[MaxLength(20)]
		public string Level { get; set; } = "Info";

		[MaxLength(100)]
		public string Event { get; set; } = string.Empty;

		[MaxLength(1000)]
		public string Message { get; set; } = string.Empty;

		[MaxLength(100)]
		public string Source { get; set; } = string.Empty;

		public string? UserId { get; set; }
		public ApplicationUser? User { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public string? MetadataJson { get; set; }
	}
}


