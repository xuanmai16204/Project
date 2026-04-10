using System;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class UserRole
	{
		public int Id { get; set; }

		public int RoleId { get; set; }
		public Role? Role { get; set; }

		public string UserId { get; set; } = string.Empty;
		public ApplicationUser? User { get; set; }

		public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
	}
}


