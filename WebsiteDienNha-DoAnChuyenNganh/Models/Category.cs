using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class Category
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[StringLength(200)]
		public string Name { get; set; } = string.Empty;

		[StringLength(200)]
		public string? Slug { get; set; }

		[StringLength(2000)]
		public string? Description { get; set; }

		public bool IsActive { get; set; } = true;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}


