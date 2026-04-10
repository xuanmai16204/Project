using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class Order
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; } = string.Empty;
		public ApplicationUser? User { get; set; }

		public DateTime OrderDate { get; set; } = DateTime.UtcNow;

	[StringLength(50)]
	public string Status { get; set; } = "Pending";

	[Column(TypeName = "decimal(18,2)")]
	public decimal Total { get; set; }

	public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.VietQR;

		[StringLength(500)]
		public string? ShippingAddress { get; set; }

		public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

		public ShippingInfo? ShippingInfo { get; set; }

		public ICollection<PromotionUsage> PromotionUsages { get; set; } = new List<PromotionUsage>();
	}
}


