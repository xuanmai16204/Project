using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class Product
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(200)]
		public string Name { get; set; } = string.Empty;

		[StringLength(200)]
		public string? Slug { get; set; }

		[StringLength(4000)]
		public string? Description { get; set; }

	[Column(TypeName = "decimal(18,2)")]
	public decimal Price { get; set; }

	[Column(TypeName = "decimal(18,2)")]
	public decimal? PromotionPrice { get; set; }

	public int Stock { get; set; }

		public bool IsActive { get; set; } = true;

		[StringLength(1000)]
		public string? ImageUrl { get; set; }

		public int CategoryId { get; set; }
		public Category? Category { get; set; }

		public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
		public ICollection<PromotionProduct> PromotionProducts { get; set; } = new List<PromotionProduct>();
		public ICollection<CustomerFeedback> Feedbacks { get; set; } = new List<CustomerFeedback>();

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdatedAt { get; set; }
	}
}
