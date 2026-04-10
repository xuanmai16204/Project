using System.Collections.Generic;

namespace WebsiteDienNha_DoAnChuyenNganh.DTO.Shipping.GoogleMaps
{
	public class DistanceMatrixResponse
	{
		public string Status { get; set; } = string.Empty;
		public List<Row> Rows { get; set; } = new();

		public class Row
		{
			public List<Element> Elements { get; set; } = new();
		}

		public class Element
		{
			public ValueText Distance { get; set; } = new();
			public ValueText Duration { get; set; } = new();
			public string Status { get; set; } = string.Empty;
		}

		public class ValueText
		{
			public string Text { get; set; } = string.Empty;
			public int Value { get; set; }
		}
	}
}


