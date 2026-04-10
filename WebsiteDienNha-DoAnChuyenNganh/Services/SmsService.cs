using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebsiteDienNha_DoAnChuyenNganh.Services
{
	public class SmsService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly ILogger<SmsService> _logger;

		public SmsService(
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration,
			ILogger<SmsService> logger)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_logger = logger;
		}

		public async Task<bool> SendOtpAsync(string phoneNumber, string otpCode)
		{
			try
			{
				// Lấy cấu hình SMS từ appsettings
				var smsProvider = _configuration["SMS:Provider"] ?? "Console";
				var apiKey = _configuration["SMS:ApiKey"];
				var apiSecret = _configuration["SMS:ApiSecret"];
				var senderId = _configuration["SMS:SenderId"] ?? "OTP";

				// Trong môi trường development, chỉ log ra console
				if (smsProvider == "Console" || string.IsNullOrEmpty(apiKey))
				{
					_logger.LogInformation($"OTP Code for {phoneNumber}: {otpCode}");
					Console.WriteLine($"\n=== SMS OTP ===\nPhone: {phoneNumber}\nOTP: {otpCode}\n==============\n");
					return true;
				}

				// Tích hợp với các nhà cung cấp SMS phổ biến ở Việt Nam
				switch (smsProvider.ToLower())
				{
					case "esms":
						return await SendViaESMSAsync(phoneNumber, otpCode, apiKey, apiSecret, senderId);
					case "brandsms":
						return await SendViaBrandSMSAsync(phoneNumber, otpCode, apiKey, apiSecret, senderId);
					case "twilio":
						return await SendViaTwilioAsync(phoneNumber, otpCode, apiKey, apiSecret, senderId);
					default:
						_logger.LogWarning($"Unknown SMS provider: {smsProvider}. Using console mode.");
						_logger.LogInformation($"OTP Code for {phoneNumber}: {otpCode}");
						return true;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error sending OTP to {phoneNumber}");
				return false;
			}
		}

		private async Task<bool> SendViaESMSAsync(string phoneNumber, string otpCode, string apiKey, string apiSecret, string senderId)
		{
			// Tích hợp với ESMS.vn - Sử dụng API POST JSON
			var client = _httpClientFactory.CreateClient();
			var message = $"Ma xac thuc cua ban la: {otpCode}. Ma co hieu luc trong 5 phut.";
			
			// Chuẩn hóa số điện thoại (đảm bảo format 84xxxxxxxxx)
			var normalizedPhone = phoneNumber.Trim();
			if (normalizedPhone.StartsWith("0"))
			{
				normalizedPhone = "84" + normalizedPhone.Substring(1);
			}
			else if (!normalizedPhone.StartsWith("84"))
			{
				normalizedPhone = "84" + normalizedPhone;
			}

			// Tạo JSON payload theo API documentation
			var payload = new
			{
				ApiKey = apiKey,
				Content = message,
				Phone = normalizedPhone,
				SecretKey = apiSecret,
				SmsType = 8, // SmsType 8 thường dùng cho OTP/CSKH
				IsUnicode = 0, // 0 = không unicode, 1 = unicode
				Sandbox = 0, // 0 = gửi thật, 1 = sandbox (test)
				campaignid = (string?)null,
				RequestId = (string?)null,
				CallbackUrl = (string?)null,
				SendDate = (string?)null
			};

			var json = JsonSerializer.Serialize(payload);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			
			var response = await client.PostAsync("https://rest.esms.vn/MainService.svc/json/SendMultipleMessage_V4_post_json/", content);
			
			if (response.IsSuccessStatusCode)
			{
				var responseContent = await response.Content.ReadAsStringAsync();
				_logger.LogInformation($"ESMS Response: {responseContent}");
				
				// Kiểm tra response có thành công không
				try
				{
					var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
					if (result.TryGetProperty("CodeResult", out var codeResult))
					{
						var code = codeResult.GetString();
						// CodeResult thường là "100" cho thành công
						return code == "100" || code == "200";
					}
				}
				catch
				{
					// Nếu không parse được JSON, coi như thành công nếu status code OK
					return true;
				}
			}
			
			return false;
		}

		private async Task<bool> SendViaBrandSMSAsync(string phoneNumber, string otpCode, string apiKey, string apiSecret, string senderId)
		{
			// Tích hợp với BrandSMS
			var client = _httpClientFactory.CreateClient();
			var message = $"Ma xac thuc cua ban la: {otpCode}. Ma co hieu luc trong 5 phut.";
			
			var payload = new
			{
				api_key = apiKey,
				api_secret = apiSecret,
				to = phoneNumber,
				content = message,
				brandname = senderId
			};

			var json = JsonSerializer.Serialize(payload);
			var content = new StringContent(json, Encoding.UTF8, "application/json");
			
			var response = await client.PostAsync("https://api.brandsms.vn/api/send", content);
			return response.IsSuccessStatusCode;
		}

		private async Task<bool> SendViaTwilioAsync(string phoneNumber, string otpCode, string apiKey, string apiSecret, string senderId)
		{
			// Tích hợp với Twilio
			var client = _httpClientFactory.CreateClient();
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
				"Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:{apiSecret}")));

			var message = $"Your verification code is: {otpCode}. Valid for 5 minutes.";
			var payload = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("From", senderId),
				new KeyValuePair<string, string>("To", phoneNumber),
				new KeyValuePair<string, string>("Body", message)
			});

			var response = await client.PostAsync($"https://api.twilio.com/2010-04-01/Accounts/{apiKey}/Messages.json", payload);
			return response.IsSuccessStatusCode;
		}
	}
}

