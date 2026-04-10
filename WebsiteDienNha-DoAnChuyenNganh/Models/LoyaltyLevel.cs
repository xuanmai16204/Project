using System.ComponentModel.DataAnnotations;

namespace WebsiteDienNha_DoAnChuyenNganh.Models
{
	public class LoyaltyLevel
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[StringLength(100)]
		public string Name { get; set; } = string.Empty;
		[Range(0, 100)]
		public int DiscountPercent { get; set; }
		public int ThresholdPoints { get; set; }
	}
}


