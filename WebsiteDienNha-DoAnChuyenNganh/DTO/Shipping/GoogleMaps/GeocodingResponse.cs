using System.Collections.Generic;

namespace WebsiteDienNha_DoAnChuyenNganh.DTO.Shipping.GoogleMaps
{
	public class GeocodingResponse
	{
		public string Status { get; set; } = string.Empty;
		public List<Result> Results { get; set; } = new();

		public class Result
		{
			public string FormattedAddress { get; set; } = string.Empty;
			public Geometry Geometry { get; set; } = new();
		}

		public class Geometry
		{
			public Location Location { get; set; } = new();
		}

		public class Location
		{
			public double Lat { get; set; }
			public double Lng { get; set; }
		}
	}
}


