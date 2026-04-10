using System;

namespace WebsiteDienNha_DoAnChuyenNganh.DTO
{
	public class SystemLogDTO
	{
		public string Level { get; set; } = string.Empty;
		public string Event { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public string Source { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public string? User { get; set; }
	}
}


