using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class Role
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(250)]
		public string? Description { get; set; }

		public bool IsSystemRole { get; set; } = true;

		public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
		public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
	}
}


