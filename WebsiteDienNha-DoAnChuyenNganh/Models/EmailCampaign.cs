using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class EmailCampaign
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(150)]
		public string Subject { get; set; } = string.Empty;

		public string Content { get; set; } = string.Empty;

		public DateTime? ScheduledAt { get; set; }

		[MaxLength(50)]
		public string Status { get; set; } = "Draft";

		public string? CreatedByUserId { get; set; }
		public ApplicationUser? CreatedByUser { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public ICollection<EmailRecipient> Recipients { get; set; } = new List<EmailRecipient>();
	}
}


