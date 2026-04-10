using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class ChatMessage
	{
		public int Id { get; set; }

		public int ConversationId { get; set; }
		public ChatConversation? Conversation { get; set; }

		[MaxLength(20)]
		public string Sender { get; set; } = "Bot";

		[MaxLength(1000)]
		public string Content { get; set; } = string.Empty;

		public bool IsFromBot { get; set; }

		public DateTime SentAt { get; set; } = DateTime.UtcNow;
	}
}


