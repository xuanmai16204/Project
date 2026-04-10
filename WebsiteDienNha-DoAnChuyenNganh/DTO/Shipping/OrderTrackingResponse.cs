using System;
using System.Collections.Generic;

namespace WebsiteDienNha_DoAnChuyenNganh.DTO.Shipping
{
	public class OrderTrackingResponse
	{
		public string TrackingNumber { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public DateTime? EstimatedDelivery { get; set; }
		public DateTime? LastUpdatedAt { get; set; }
		public List<TrackingEvent> Events { get; set; } = new();

		public class TrackingEvent
		{
			public DateTime Time { get; set; }
			public string Description { get; set; } = string.Empty;
			public string Location { get; set; } = string.Empty;
		}
	}
}
