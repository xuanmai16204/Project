using System.ComponentModel.DataAnnotations;
using WebsiteDienNha_DoAnChuyenNganh.Models;

namespace WebsiteDienNha_DoAnChuyenNganh.DTO
{
	public class CheckoutViewModel
	{
		public List<(CartItem Item, Product? Product)> CartItems { get; set; } = new();
		public decimal Total { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập họ tên")]
		[Display(Name = "Họ và tên")]
		public string FullName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
		[Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
		[Display(Name = "Số điện thoại")]
		public string PhoneNumber { get; set; } = string.Empty;

		[Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
		[Display(Name = "Địa chỉ giao hàng")]
		public string ShippingAddress { get; set; } = string.Empty;

		[Display(Name = "Ghi chú")]
		public string? Note { get; set; }

		[Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
		[Display(Name = "Phương thức thanh toán")]
		public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.VietQR;
	}
}

