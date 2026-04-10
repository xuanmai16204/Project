using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class Permission
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		[MaxLength(250)]
		public string? Description { get; set; }

		public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
	}
}


