using Microsoft.Extensions.Options;
using WebsiteDienNha_DoAnChuyenNganh.Config;

namespace WebsiteDienNha_DoAnChuyenNganh.Services
{
	public class VietQRService
	{
		private readonly VietQRSetting _settings;
		private readonly ILogger<VietQRService> _logger;

		public VietQRService(
			IOptions<VietQRSetting> settings,
			ILogger<VietQRService> logger)
		{
			_settings = settings.Value;
			_logger = logger;
		}

		/// <summary>
		/// Tạo URL QR code từ VietQR.io
		/// Format: https://img.vietqr.io/image/{bank}-{account}-{template}.jpg
		/// </summary>
		public string GenerateQRCodeUrl(decimal? amount = null, string? addInfo = null)
		{
			var bankCode = _settings.BankCode.ToLower();
			var accountNumber = _settings.AccountNumber;
			var template = _settings.Template;
			var finalAmount = amount ?? _settings.Amount;
			var finalAddInfo = addInfo ?? _settings.AddInfo;

			// Tạo base URL
			var baseUrl = $"https://img.vietqr.io/image/{bankCode}-{accountNumber}-{template}.jpg";

			// Thêm query parameters nếu có
			var queryParams = new List<string>();
			
			if (finalAmount.HasValue && finalAmount.Value > 0)
			{
				queryParams.Add($"amount={finalAmount.Value}");
			}

			if (!string.IsNullOrEmpty(finalAddInfo))
			{
				queryParams.Add($"addInfo={Uri.EscapeDataString(finalAddInfo)}");
			}

			if (queryParams.Any())
			{
				baseUrl += "?" + string.Join("&", queryParams);
			}

			return baseUrl;
		}

		/// <summary>
		/// Tạo Quicklink URL từ VietQR.io (nếu có API key)
		/// </summary>
		public string GenerateQuicklinkUrl(decimal amount, string addInfo, int orderId)
		{
			// Format: https://img.vietqr.io/image/{bank}-{account}-{template}.jpg?amount={amount}&addInfo={addInfo}
			var addInfoWithOrder = $"{addInfo} - Don hang #{orderId}";
			return GenerateQRCodeUrl(amount, addInfoWithOrder);
		}

		/// <summary>
		/// Tạo Deeplink để mở app ngân hàng
		/// </summary>
		public string GenerateDeeplink(decimal amount, string addInfo, int orderId)
		{
			var bankCode = _settings.BankCode.ToLower();
			var accountNumber = _settings.AccountNumber;
			var addInfoWithOrder = $"{addInfo} - Don hang #{orderId}";

			// Format deeplink theo từng ngân hàng
			switch (bankCode)
			{
				case "vietinbank":
					return $"vietinbank://transfer?account={accountNumber}&amount={amount}&content={Uri.EscapeDataString(addInfoWithOrder)}";
				case "vietcombank":
					return $"vietcombank://transfer?account={accountNumber}&amount={amount}&content={Uri.EscapeDataString(addInfoWithOrder)}";
				case "techcombank":
					return $"techcombank://transfer?account={accountNumber}&amount={amount}&content={Uri.EscapeDataString(addInfoWithOrder)}";
				default:
					// Fallback về QR code URL
					return GenerateQRCodeUrl(amount, addInfoWithOrder);
			}
		}

		/// <summary>
		/// Lấy thông tin ngân hàng để hiển thị
		/// </summary>
		public (string BankName, string AccountNumber, string AccountName) GetBankInfo()
		{
			var bankNames = new Dictionary<string, string>
			{
				{ "vietinbank", "VietinBank" },
				{ "vietcombank", "Vietcombank" },
				{ "techcombank", "Techcombank" },
				{ "bidv", "BIDV" },
				{ "acb", "ACB" },
				{ "tpb", "TPBank" },
				{ "mbbank", "MB Bank" },
				{ "vpbank", "VPBank" },
				{ "sacombank", "Sacombank" }
			};

			var bankCode = _settings.BankCode.ToLower();
			var bankName = bankNames.ContainsKey(bankCode) ? bankNames[bankCode] : _settings.BankCode.ToUpper();

			return (bankName, _settings.AccountNumber, _settings.AccountName);
		}
	}
}

