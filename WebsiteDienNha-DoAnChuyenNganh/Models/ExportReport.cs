using System;
using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class ExportReport
	{
		public int Id { get; set; }

		[MaxLength(150)]
		public string FileName { get; set; } = string.Empty;

		[MaxLength(250)]
		public string FilePath { get; set; } = string.Empty;

		[MaxLength(100)]
		public string ContentType { get; set; } = "application/pdf";

		public long SizeInBytes { get; set; }

		public string? GeneratedByUserId { get; set; }
		public ApplicationUser? GeneratedByUser { get; set; }

		[MaxLength(50)]
		public string ReportType { get; set; } = "Sales";

		public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
	}
}


