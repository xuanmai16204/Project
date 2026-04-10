using System;
using System.Linq;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.Utils
{
	public static class ProductHelper
	{
	/// <summary>
	/// Tính giá khuyến mãi của sản phẩm dựa trên các promotion đang hoạt động
	/// Ưu tiên sử dụng PromotionPrice nếu có, nếu không thì tính từ Promotion
	/// </summary>
	/// <param name="product">Sản phẩm cần tính giá</param>
	/// <returns>Giá sau khi giảm, nếu không có promotion thì trả về giá gốc</returns>
	public static decimal GetPromotionPrice(Product product)
	{
		if (product == null)
		{
			return 0;
		}

		// Nếu có PromotionPrice được set trực tiếp, ưu tiên sử dụng
		if (product.PromotionPrice.HasValue && product.PromotionPrice.Value > 0 && product.PromotionPrice.Value < product.Price)
		{
			return product.PromotionPrice.Value;
		}

		// Nếu không có PromotionPrice, tính từ Promotion
		if (product.PromotionProducts == null || !product.PromotionProducts.Any())
		{
			return product.Price;
		}

			var now = DateTime.UtcNow;
			var activePromotion = product.PromotionProducts
				.Where(pp => pp.Promotion != null 
					&& pp.Promotion.IsActive 
					&& pp.Promotion.StartDate <= now 
					&& pp.Promotion.EndDate >= now)
				.Select(pp => pp.Promotion!)
				.OrderByDescending(p => p.DiscountPercent ?? 0)
				.ThenByDescending(p => p.DiscountAmount ?? 0)
				.FirstOrDefault();

			if (activePromotion == null)
			{
				return product.Price;
			}

			decimal discountAmount = 0;

			// Tính giảm giá theo phần trăm
			// Ví dụ: Giá gốc 600.000, giảm 50% => Giảm 300.000 => Giá còn 300.000
			if (activePromotion.DiscountPercent.HasValue && activePromotion.DiscountPercent.Value > 0)
			{
				// Tính số tiền được giảm: Giá gốc * (Phần trăm / 100)
				discountAmount = product.Price * (activePromotion.DiscountPercent.Value / 100m);
				
				// Áp dụng giới hạn tối đa nếu có (ví dụ: giảm tối đa 100.000)
				if (activePromotion.MaxDiscount.HasValue && discountAmount > activePromotion.MaxDiscount.Value)
				{
					discountAmount = activePromotion.MaxDiscount.Value;
				}
			}
			// Tính giảm giá theo số tiền cố định (ví dụ: giảm 50.000)
			else if (activePromotion.DiscountAmount.HasValue && activePromotion.DiscountAmount.Value > 0)
			{
				discountAmount = activePromotion.DiscountAmount.Value;
			}

			// Giá cuối cùng = Giá gốc - Số tiền được giảm
			// Ví dụ: 600.000 - 300.000 = 300.000
			var finalPrice = product.Price - discountAmount;
			return finalPrice > 0 ? finalPrice : 0;
		}

		/// <summary>
		/// Lấy thông tin promotion đang áp dụng cho sản phẩm
		/// </summary>
		/// <param name="product">Sản phẩm</param>
		/// <returns>Promotion đang hoạt động, null nếu không có</returns>
		public static Promotion? GetActivePromotion(Product product)
		{
			if (product == null || product.PromotionProducts == null || !product.PromotionProducts.Any())
			{
				return null;
			}

			var now = DateTime.UtcNow;
			return product.PromotionProducts
				.Where(pp => pp.Promotion != null 
					&& pp.Promotion.IsActive 
					&& pp.Promotion.StartDate <= now 
					&& pp.Promotion.EndDate >= now)
				.Select(pp => pp.Promotion!)
				.OrderByDescending(p => p.DiscountPercent ?? 0)
				.ThenByDescending(p => p.DiscountAmount ?? 0)
				.FirstOrDefault();
		}

		/// <summary>
		/// Kiểm tra sản phẩm có đang được khuyến mãi không
		/// </summary>
		/// <param name="product">Sản phẩm</param>
		/// <returns>True nếu có promotion đang hoạt động</returns>
		public static bool HasActivePromotion(Product product)
		{
			return GetActivePromotion(product) != null;
		}

		/// <summary>
		/// Tính phần trăm giảm giá
		/// </summary>
		/// <param name="product">Sản phẩm</param>
		/// <returns>Phần trăm giảm giá (0-100)</returns>
		public static decimal GetDiscountPercent(Product product)
		{
			if (product == null || product.Price == 0)
			{
				return 0;
			}

			var promotionPrice = GetPromotionPrice(product);
			if (promotionPrice >= product.Price)
			{
				return 0;
			}

			var discount = product.Price - promotionPrice;
			return Math.Round((discount / product.Price) * 100, 0);
		}
	}
}

