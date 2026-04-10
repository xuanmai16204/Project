using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using WebsiteDienNha_DoAnChuyenNganh.Config;

namespace WebsiteDienNha_DoAnChuyenNganh.Utils
{
	public class PayLib
	{
		private readonly VNPaySetting _settings;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public PayLib(IOptions<VNPaySetting> settings, IHttpContextAccessor httpContextAccessor)
		{
			_settings = settings.Value;
			_httpContextAccessor = httpContextAccessor;
		}

		public string CreatePaymentUrl(decimal amount, string orderInfo, int orderId)
		{
			var vnp_TmnCode = _settings.TmnCode;
			var vnp_HashSecret = _settings.HashSecret;
			var vnp_Url = _settings.BaseUrl;
			var vnp_ReturnUrl = _settings.ReturnUrl;

			var vnp_TxnRef = orderId.ToString();
			var vnp_OrderInfo = orderInfo;
			var vnp_OrderType = "other";
			var vnp_Amount = (long)(amount * 100);
			var vnp_Locale = "vn";
			var vnp_IpAddr = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
			var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");

			var vnp_Params = new Dictionary<string, string>
			{
				{ "vnp_Version", "2.1.0" },
				{ "vnp_Command", "pay" },
				{ "vnp_TmnCode", vnp_TmnCode },
				{ "vnp_Locale", vnp_Locale },
				{ "vnp_CurrCode", "VND" },
				{ "vnp_TxnRef", vnp_TxnRef },
				{ "vnp_OrderInfo", vnp_OrderInfo },
				{ "vnp_OrderType", vnp_OrderType },
				{ "vnp_Amount", vnp_Amount.ToString() },
				{ "vnp_ReturnUrl", vnp_ReturnUrl },
				{ "vnp_IpAddr", vnp_IpAddr },
				{ "vnp_CreateDate", vnp_CreateDate }
			};

			var queryString = string.Join("&", vnp_Params.OrderBy(x => x.Key).Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
			var signData = queryString;
			var vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);

			return $"{vnp_Url}?{queryString}&vnp_SecureHash={vnp_SecureHash}";
		}

		private string HmacSHA512(string key, string inputData)
		{
			var hash = new StringBuilder();
			byte[] keyBytes = Encoding.UTF8.GetBytes(key);
			byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
			using (var hmac = new HMACSHA512(keyBytes))
			{
				byte[] hashValue = hmac.ComputeHash(inputBytes);
				foreach (var theByte in hashValue)
				{
					hash.Append(theByte.ToString("x2"));
				}
			}
			return hash.ToString();
		}
	}
}


