using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class GuestNotification
	{
		public int Id { get; set; }

		[MaxLength(150)]
		public string Contact { get; set; } = string.Empty;

		[MaxLength(1000)]
		public string Message { get; set; } = string.Empty;

		[MaxLength(50)]
		public string Channel { get; set; } = "Email";

		[MaxLength(50)]
		public string Status { get; set; } = "New";

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}


