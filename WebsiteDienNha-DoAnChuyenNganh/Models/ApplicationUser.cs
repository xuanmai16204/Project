using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class ApplicationUser : IdentityUser
	{
		[MaxLength(100)]
		public string FullName { get; set; } = string.Empty;

		[MaxLength(250)]
		public string? AvatarUrl { get; set; }

		[MaxLength(250)]
		public string? Address { get; set; }

		[MaxLength(100)]
		public string? City { get; set; }

		[MaxLength(100)]
		public string? District { get; set; }

		[MaxLength(100)]
		public string? Ward { get; set; }

		public int LoyaltyPoints { get; set; }

		public int LoyaltyLevelId { get; set; } = 1;
		public LoyaltyLevel? LoyaltyLevel { get; set; }

		public ICollection<RewardPointTransaction> RewardTransactions { get; set; } = new List<RewardPointTransaction>();

		public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

		public ICollection<ShippingInfo> ShippingInfos { get; set; } = new List<ShippingInfo>();

		public Employee? EmployeeProfile { get; set; }
	}
}

