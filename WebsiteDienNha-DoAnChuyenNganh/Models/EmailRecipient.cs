using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class EmailRecipient
	{
		public int Id { get; set; }

		public int CampaignId { get; set; }
		public EmailCampaign? Campaign { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[MaxLength(50)]
		public string Status { get; set; } = "Pending";

		public DateTime? SentAt { get; set; }
		public DateTime? OpenedAt { get; set; }
		public DateTime? ClickedAt { get; set; }
	}
}


