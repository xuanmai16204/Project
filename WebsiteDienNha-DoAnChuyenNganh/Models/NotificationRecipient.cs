using System;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class NotificationRecipient
	{
		public int Id { get; set; }

		public int SystemNotificationId { get; set; }
		public SystemNotification? SystemNotification { get; set; }

		public string UserId { get; set; } = string.Empty;
		public ApplicationUser? User { get; set; }

		public bool IsRead { get; set; }
		public DateTime? ReadAt { get; set; }
	}
}


