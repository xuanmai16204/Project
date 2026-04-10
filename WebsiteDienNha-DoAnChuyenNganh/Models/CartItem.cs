using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class CartItem
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public Guid CartId { get; set; }
		public Cart? Cart { get; set; }

		[Required]
		public int ProductId { get; set; }
		public Product? Product { get; set; }

		[Range(1, int.MaxValue)]
		public int Quantity { get; set; } = 1;

		[Column(TypeName = "decimal(18,2)")]
		public decimal UnitPrice { get; set; }
	}
}


