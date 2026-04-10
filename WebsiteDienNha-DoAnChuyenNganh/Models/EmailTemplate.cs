using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class EmailTemplate
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(150)]
		public string Subject { get; set; } = string.Empty;

		public string HtmlContent { get; set; } = string.Empty;

		public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
	}
}


