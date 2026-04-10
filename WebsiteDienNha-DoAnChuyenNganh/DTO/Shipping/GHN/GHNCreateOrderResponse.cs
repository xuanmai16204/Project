using System;
using System.Collections.Generic;

namespace WebsiteDienNha_DoAnChuyenNganh.DTO.Shipping.GHN
{
	public class GHNCreateOrderResponse
	{
		public string OrderCode { get; set; } = string.Empty;
		public string TrackingNumber { get; set; } = string.Empty;
		public decimal TotalFee { get; set; }
		public DateTime? EstimatedDelivery { get; set; }
		public List<string> Notes { get; set; } = new();
	}
}


