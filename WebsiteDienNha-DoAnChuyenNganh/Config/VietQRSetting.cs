namespace WebsiteDienNha_DoAnChuyenNganh.Config
{
	public class VietQRSetting
	{
		// Tên ngân hàng (ví dụ: vietinbank, vietcombank, techcombank, etc.)
		public string BankCode { get; set; } = "vietinbank";
		
		// Số tài khoản ngân hàng
		public string AccountNumber { get; set; } = "113366668888";
		
		// Tên chủ tài khoản
		public string AccountName { get; set; } = "CONG TY DIEN NHA";
		
		// Template QR code: compact, compact2, qr_only, print
		public string Template { get; set; } = "compact";
		
		// Số tiền (nếu có)
		public decimal? Amount { get; set; }
		
		// Nội dung chuyển khoản
		public string? AddInfo { get; set; }
	}
}

