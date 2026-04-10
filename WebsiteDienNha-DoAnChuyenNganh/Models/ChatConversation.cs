using System;
using System.Collections.Generic;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class ChatConversation
	{
		public int Id { get; set; }

		public string? UserId { get; set; }
		public ApplicationUser? User { get; set; }

		public DateTime StartedAt { get; set; } = DateTime.UtcNow;

		public DateTime? ClosedAt { get; set; }

		public string Status { get; set; } = "Open";

		public string? Topic { get; set; }

		public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
	}
}


