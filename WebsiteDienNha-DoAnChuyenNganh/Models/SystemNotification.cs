using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class SystemNotification
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(150)]
		public string Title { get; set; } = string.Empty;

		[MaxLength(1000)]
		public string Message { get; set; } = string.Empty;

		[MaxLength(50)]
		public string TargetRole { get; set; } = "All";

		[MaxLength(20)]
		public string Priority { get; set; } = "Normal";

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime? ExpiresAt { get; set; }

		public string? PublishedByUserId { get; set; }
		public ApplicationUser? PublishedByUser { get; set; }

		public ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
	}
}


