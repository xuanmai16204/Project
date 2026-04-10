using Microsoft.Extensions.Caching.Memory;

namespace WebsiteDienNha_DoAnChuyenNganh.Services
{
	public class OtpService
	{
		private readonly IMemoryCache _cache;
		private readonly ILogger<OtpService> _logger;
		private const int OTP_EXPIRY_MINUTES = 5;
		private const int OTP_LENGTH = 6;

		public OtpService(IMemoryCache cache, ILogger<OtpService> logger)
		{
			_cache = cache;
			_logger = logger;
		}

		public string GenerateOtp()
		{
			var random = new Random();
			var otp = random.Next(100000, 999999).ToString();
			return otp;
		}

		public void StoreOtp(string phoneNumber, string otp)
		{
			var cacheKey = $"OTP_{phoneNumber}";
			var cacheOptions = new MemoryCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(OTP_EXPIRY_MINUTES),
				SlidingExpiration = TimeSpan.FromMinutes(1)
			};

			_cache.Set(cacheKey, otp, cacheOptions);
			_logger.LogInformation($"OTP stored for {phoneNumber}, expires in {OTP_EXPIRY_MINUTES} minutes");
		}

		public bool VerifyOtp(string phoneNumber, string otp)
		{
			var cacheKey = $"OTP_{phoneNumber}";
			
			if (!_cache.TryGetValue(cacheKey, out string? storedOtp))
			{
				_logger.LogWarning($"OTP not found or expired for {phoneNumber}");
				return false;
			}

			if (storedOtp != otp)
			{
				_logger.LogWarning($"Invalid OTP for {phoneNumber}");
				return false;
			}

			// Xóa OTP sau khi verify thành công
			_cache.Remove(cacheKey);
			_logger.LogInformation($"OTP verified successfully for {phoneNumber}");
			return true;
		}

		public bool HasOtp(string phoneNumber)
		{
			var cacheKey = $"OTP_{phoneNumber}";
			return _cache.TryGetValue(cacheKey, out _);
		}

		public int GetRemainingAttempts(string phoneNumber)
		{
			var attemptsKey = $"OTP_ATTEMPTS_{phoneNumber}";
			if (_cache.TryGetValue(attemptsKey, out int attempts))
			{
				return Math.Max(0, 5 - attempts);
			}
			return 5;
		}

		public void IncrementAttempts(string phoneNumber)
		{
			var attemptsKey = $"OTP_ATTEMPTS_{phoneNumber}";
			if (_cache.TryGetValue(attemptsKey, out int attempts))
			{
				attempts++;
			}
			else
			{
				attempts = 1;
			}

			var cacheOptions = new MemoryCacheEntryOptions
			{
				AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
			};

			_cache.Set(attemptsKey, attempts, cacheOptions);
		}

		public void ResetAttempts(string phoneNumber)
		{
			var attemptsKey = $"OTP_ATTEMPTS_{phoneNumber}";
			_cache.Remove(attemptsKey);
		}
	}
}

