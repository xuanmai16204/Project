using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class Cart
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		public string UserId { get; set; } = string.Empty;
		public ApplicationUser? User { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
	}
}


