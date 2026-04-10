using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class ShippingInfo
	{
		public int Id { get; set; }

		[Required]
		public int OrderId { get; set; }
		public Order? Order { get; set; }

		[Required]
		public string UserId { get; set; } = string.Empty;
		public ApplicationUser? User { get; set; }

		[MaxLength(100)]
		public string Carrier { get; set; } = "GHN";

		[MaxLength(100)]
		public string TrackingNumber { get; set; } = string.Empty;

		[MaxLength(50)]
		public string Status { get; set; } = "Preparing";

		public DateTime? EstimatedDelivery { get; set; }

		[MaxLength(300)]
		public string AddressLine { get; set; } = string.Empty;

		[MaxLength(100)]
		public string Province { get; set; } = string.Empty;

		[MaxLength(100)]
		public string District { get; set; } = string.Empty;

		[MaxLength(100)]
		public string Ward { get; set; } = string.Empty;

		[Column(TypeName = "decimal(18,2)")]
		public decimal ShippingFee { get; set; }

		public double DistanceKm { get; set; }

		[MaxLength(200)]
		public string? Notes { get; set; }

		[MaxLength(200)]
		public string? ShippingService { get; set; }
	}
}


