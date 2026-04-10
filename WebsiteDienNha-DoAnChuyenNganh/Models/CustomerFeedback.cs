using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class CustomerFeedback
	{
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; } = string.Empty;
		public ApplicationUser? User { get; set; }

		public int? OrderId { get; set; }
		public Order? Order { get; set; }

		public int? ProductId { get; set; }
		public Product? Product { get; set; }

		[Range(1, 5)]
		public int Rating { get; set; }

		[MaxLength(1000)]
		public string? Comment { get; set; }

		[MaxLength(50)]
		public string Status { get; set; } = "Pending";

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}


