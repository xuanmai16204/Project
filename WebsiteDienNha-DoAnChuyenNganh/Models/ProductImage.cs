using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class ProductImage
	{
		public int Id { get; set; }

		[Required]
		public int ProductId { get; set; }
		public Product? Product { get; set; }

		[Required]
		[MaxLength(500)]
		public string ImageUrl { get; set; } = string.Empty;

		[MaxLength(200)]
		public string? AltText { get; set; }

		public int SortOrder { get; set; } = 0;

		public bool IsPrimary { get; set; } = false;
	}
}


