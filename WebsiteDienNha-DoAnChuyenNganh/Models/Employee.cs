using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class Employee
	{
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; } = string.Empty;
		public ApplicationUser? User { get; set; }

		[MaxLength(20)]
		public string EmployeeCode { get; set; } = string.Empty;

		[MaxLength(100)]
		public string Department { get; set; } = string.Empty;

		[MaxLength(100)]
		public string Title { get; set; } = string.Empty;

		public DateTime HireDate { get; set; } = DateTime.UtcNow;

		[MaxLength(50)]
		public string Status { get; set; } = "Active";

		[MaxLength(100)]
		public string? ManagerName { get; set; }

		[MaxLength(50)]
		public string? SalaryGrade { get; set; }

		public ICollection<EmployeePerformance> Performances { get; set; } = new List<EmployeePerformance>();
	}
}


