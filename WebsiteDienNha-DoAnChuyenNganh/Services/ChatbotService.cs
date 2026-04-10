using Microsoft.EntityFrameworkCore;
using WebsiteDienNha_DoAnChuyenNganh.Data;
using WebsiteDienNha_DoAnChuyenNganh.Models;
using System.Text;
using System.Text.Json;
using WebsiteDienNha_DoAnChuyenNganh.IRepository;

namespace WebsiteDienNha_DoAnChuyenNganh.Services
{
	public class ChatbotService
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;
		private readonly ILogger<ChatbotService> _logger;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IProductRepository _productRepository;

		public ChatbotService(
			ApplicationDbContext context,
			IConfiguration configuration,
			ILogger<ChatbotService> logger,
			IHttpClientFactory httpClientFactory,
			IProductRepository productRepository)
		{
			_context = context;
			_configuration = configuration;
			_logger = logger;
			_httpClientFactory = httpClientFactory;
			_productRepository = productRepository;
		}

		public async Task<string> GetResponseAsync(string userMessage, string? userId, int? conversationId = null)
		{
			ChatConversation? conversation = null;
			
			// Tạo response trước, không phụ thuộc vào database
			string botResponse;
			try
			{
				// Lấy conversation nếu có
				if (conversationId.HasValue)
				{
					try
					{
						conversation = await _context.ChatConversations
							.Include(c => c.Messages)
							.FirstOrDefaultAsync(c => c.Id == conversationId);
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, "Could not load conversation {ConversationId}", conversationId);
					}
				}

				// Tạo response (conversation có thể null)
				botResponse = await GenerateBotResponseAsync(userMessage, conversation ?? new ChatConversation());
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error generating bot response: {Message}", ex.Message);
				// Trả về response mặc định thân thiện
				botResponse = "Xin chào! 👋 Tôi là nhân viên tư vấn của Điện Nhà. Tôi có thể giúp bạn tìm sản phẩm, đặt hàng hoặc giải đáp thắc mắc. Bạn cần hỗ trợ gì ạ?";
			}

			// Lưu vào database (không bắt buộc, nếu lỗi vẫn trả về response)
			try
			{
				if (conversation == null)
				{
					conversation = new ChatConversation
					{
						UserId = userId,
						Status = "Open",
						StartedAt = DateTime.UtcNow
					};
					_context.ChatConversations.Add(conversation);
					await _context.SaveChangesAsync();
				}

				// Lưu tin nhắn của user
				var userMsg = new ChatMessage
				{
					ConversationId = conversation.Id,
					Sender = userId ?? "Guest",
					Content = userMessage,
					IsFromBot = false,
					SentAt = DateTime.UtcNow
				};
				_context.ChatMessages.Add(userMsg);

				// Lưu tin nhắn của bot
				var botMsg = new ChatMessage
				{
					ConversationId = conversation.Id,
					Sender = "Bot",
					Content = botResponse,
					IsFromBot = true,
					SentAt = DateTime.UtcNow
				};
				_context.ChatMessages.Add(botMsg);
				
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex, "Could not save conversation to database, but response was generated");
				// Vẫn trả về response dù không lưu được vào database
			}

			return botResponse;
		}

		private async Task<string> GenerateBotResponseAsync(string userMessage, ChatConversation conversation)
		{
			try
			{
				// Luôn sử dụng rule-based để trả lời tự nhiên như nhân viên tư vấn
				var message = userMessage.ToLower().Trim();
				return await GenerateRuleBasedResponseAsync(message, conversation);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in GenerateBotResponseAsync: {Message}", ex.Message);
				// Fallback response nếu có lỗi
				return "Xin chào! Tôi là nhân viên tư vấn của Điện Nhà. Bạn cần hỗ trợ gì ạ?";
			}
		}

		private async Task<string> GenerateRuleBasedResponseAsync(string message, ChatConversation? conversation = null)
		{
			try
			{
				// Lấy lịch sử tin nhắn gần đây để có context
				var recentContext = new List<string>();
				if (conversation != null && conversation.Id > 0)
				{
					try
					{
						var recentMessages = await _context.ChatMessages
							.Where(m => m.ConversationId == conversation.Id)
							.OrderByDescending(m => m.SentAt)
							.Take(5)
							.OrderBy(m => m.SentAt)
							.ToListAsync();
						
						recentContext = recentMessages.Select(m => m.Content.ToLower()).ToList();
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, "Could not load conversation context");
						// Tiếp tục với recentContext rỗng
					}
				}

			// Chào hỏi - nhiều cách chào khác nhau
			if (IsGreeting(message))
			{
				try
				{
					var greetings = new[]
					{
						"Xin chào! 👋 Rất vui được hỗ trợ bạn! Tôi là nhân viên tư vấn của Điện Nhà. Bạn cần tư vấn về sản phẩm nào ạ?",
						"Chào bạn! 😊 Tôi có thể giúp bạn tìm sản phẩm, đặt hàng hoặc giải đáp mọi thắc mắc. Bạn muốn hỏi gì?",
						"Xin chào! Tôi sẵn sàng hỗ trợ bạn. Bạn đang tìm kiếm sản phẩm gì hoặc có câu hỏi gì cần giải đáp không?",
						"Chào mừng bạn đến với Điện Nhà! 🔌 Tôi có thể tư vấn về sản phẩm, hướng dẫn đặt hàng, hoặc giải đáp thắc mắc. Bạn cần gì ạ?"
					};
					return greetings[new Random().Next(greetings.Length)];
				}
				catch
				{
					return "Xin chào! 👋 Rất vui được hỗ trợ bạn! Tôi là nhân viên tư vấn của Điện Nhà. Bạn cần tư vấn về sản phẩm nào ạ?";
				}
			}

			// Cảm ơn
			if (message.Contains("cảm ơn") || message.Contains("thanks") || message.Contains("thank"))
			{
				return "Không có gì đâu ạ! 😊 Rất vui được hỗ trợ bạn. Nếu còn thắc mắc gì, cứ hỏi tôi nhé!";
			}

			// Tạm biệt
			if (message.Contains("tạm biệt") || message.Contains("bye") || message.Contains("goodbye") || message.Contains("chào"))
			{
				return "Cảm ơn bạn đã liên hệ! Chúc bạn một ngày tốt lành! 👋 Nếu cần hỗ trợ thêm, cứ quay lại nhé!";
			}

			// Tìm sản phẩm - Trả về dữ liệu thật
			if (IsProductSearch(message))
			{
				return await GetProductsResponseAsync(message);
			}

			// Tìm sản phẩm theo tên cụ thể
			var keywords = ExtractProductKeywords(message);
			if (keywords.Any() && (message.Contains("tìm") || message.Contains("có") || message.Contains("bán") || 
			    message.Contains("mua") || message.Contains("xem") || message.Contains("giá")))
			{
				return await SearchProductsResponseAsync(keywords);
			}

			// Danh mục sản phẩm
			if (message.Contains("danh mục") || message.Contains("loại") || message.Contains("category") || 
			    message.Contains("nhóm") || message.Contains("phân loại"))
			{
				return await GetCategoriesResponseAsync();
			}

			// Đặt hàng / Thanh toán
			if (IsOrderRelated(message))
			{
				return GetOrderResponse(message);
			}

			// Theo dõi đơn hàng
			if (message.Contains("đơn hàng") || message.Contains("theo dõi") || message.Contains("tracking") || 
			    message.Contains("trạng thái") || message.Contains("đã giao") || message.Contains("đang giao"))
			{
				return "Để theo dõi đơn hàng của bạn:\n\n" +
					"📱 **Cách 1:** Đăng nhập vào tài khoản → Vào mục 'Đơn hàng' hoặc 'Lịch sử đơn hàng'\n\n" +
					"📋 **Cách 2:** Nếu bạn có mã đơn hàng, tôi có thể hướng dẫn bạn tra cứu\n\n" +
					"Bạn đã đăng nhập chưa? Hoặc bạn có mã đơn hàng không ạ?";
			}

			// Bảo hành / Đổi trả
			if (IsWarrantyRelated(message))
			{
				return "Về chính sách bảo hành và đổi trả:\n\n" +
					"✅ **Bảo hành:**\n" +
					"• Tất cả sản phẩm đều có bảo hành chính hãng\n" +
					"• Thời gian bảo hành tùy theo từng sản phẩm (thường 12-24 tháng)\n" +
					"• Hỗ trợ bảo hành tại các trung tâm ủy quyền\n\n" +
					"🔄 **Đổi trả:**\n" +
					"• Đổi trả miễn phí trong 7 ngày đầu nếu sản phẩm lỗi\n" +
					"• Sản phẩm phải còn nguyên vẹn, chưa sử dụng\n" +
					"• Miễn phí vận chuyển đổi trả\n\n" +
					"Bạn cần hỗ trợ về sản phẩm nào cụ thể không ạ?";
			}

			// Giá cả / Khuyến mãi
			if (IsPriceRelated(message))
			{
				return await GetPriceResponseAsync(message);
			}

			// Vận chuyển / Giao hàng
			if (IsShippingRelated(message))
			{
				return "Thông tin vận chuyển và giao hàng:\n\n" +
					"🚚 **Phí vận chuyển:**\n" +
					"• Miễn phí ship cho đơn hàng từ 500.000đ trở lên\n" +
					"• Đơn hàng dưới 500.000đ: phí ship 30.000đ\n\n" +
					"📍 **Khu vực giao hàng:**\n" +
					"• Giao hàng toàn quốc\n" +
					"• Ưu tiên giao hàng tại TP.HCM và Hà Nội\n\n" +
					"⏰ **Thời gian giao hàng:**\n" +
					"• Nội thành: 1-2 ngày làm việc\n" +
					"• Tỉnh thành khác: 3-5 ngày làm việc\n" +
					"• Hỗ trợ theo dõi đơn hàng real-time qua SMS/Email\n\n" +
					"Bạn có câu hỏi gì về vận chuyển không ạ?";
			}

			// Khuyến mãi / Mã giảm giá
			if (IsPromotionRelated(message))
			{
				return "Về chương trình khuyến mãi:\n\n" +
					"🎁 **Hiện tại chúng tôi có:**\n" +
					"• Nhiều chương trình khuyến mãi theo từng sản phẩm\n" +
					"• Mã giảm giá cho khách hàng mới\n" +
					"• Chương trình tích điểm thưởng cho thành viên\n" +
					"• Flash sale vào các ngày cuối tuần\n\n" +
					"💡 **Để nhận mã giảm giá:**\n" +
					"• Đăng ký tài khoản mới nhận ngay ưu đãi 5%\n" +
					"• Theo dõi fanpage để nhận mã giảm giá độc quyền\n" +
					"• Tích điểm khi mua hàng để đổi quà\n\n" +
					"Bạn muốn xem sản phẩm đang khuyến mãi không ạ?";
			}

			// Hỗ trợ kỹ thuật
			if (IsTechnicalSupport(message))
			{
				return "Về hỗ trợ kỹ thuật:\n\n" +
					"🔧 **Chúng tôi hỗ trợ:**\n" +
					"• Tư vấn lắp đặt sản phẩm\n" +
					"• Hướng dẫn sử dụng chi tiết\n" +
					"• Xử lý sự cố kỹ thuật\n" +
					"• Bảo trì và sửa chữa\n\n" +
					"📞 **Liên hệ hỗ trợ:**\n" +
					"• Hotline: 1900 1234 (8:00 - 22:00)\n" +
					"• Email: support@diennha.vn\n" +
					"• Chat trực tuyến: Bạn đang ở đây rồi! 😊\n\n" +
					"Bạn gặp vấn đề gì với sản phẩm? Tôi có thể hướng dẫn ngay!";
			}

			// Liên hệ / Hotline
			if (IsContactRelated(message))
			{
				return "Thông tin liên hệ:\n\n" +
					"📞 **Hotline:** 1900 1234 (8:00 - 22:00 hàng ngày)\n" +
					"📧 **Email:** support@diennha.vn\n" +
					"📍 **Địa chỉ:** 123 Đường ABC, Quận XYZ, TP.HCM\n" +
					"💬 **Chat:** Bạn đang chat với tôi đây! 😊\n\n" +
					"Bạn cần hỗ trợ gì cụ thể? Tôi sẵn sàng giúp đỡ!";
			}

			// Giờ làm việc
			if (message.Contains("giờ làm việc") || message.Contains("mở cửa") || message.Contains("đóng cửa"))
			{
				return "⏰ **Giờ làm việc:**\n" +
					"• Thứ 2 - Chủ nhật: 8:00 - 22:00\n" +
					"• Hotline hỗ trợ: 8:00 - 22:00\n" +
					"• Giao hàng: Tất cả các ngày trong tuần\n\n" +
					"Chúng tôi luôn sẵn sàng phục vụ bạn! 😊";
			}

			// Câu hỏi không hiểu - trả lời thân thiện
			return GetFriendlyDefaultResponse(message, recentContext);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in GenerateRuleBasedResponseAsync: {Message}", ex.Message);
				// Trả về câu trả lời mặc định an toàn
				return "Xin chào! Tôi là nhân viên tư vấn của Điện Nhà. Tôi có thể giúp bạn tìm sản phẩm, đặt hàng hoặc giải đáp thắc mắc. Bạn cần hỗ trợ gì ạ?";
			}
		}

		// Helper methods để kiểm tra intent
		private bool IsGreeting(string message)
		{
			var greetings = new[] { "xin chào", "hello", "chào", "hi", "hey", "chào bạn", "chào anh", "chào chị" };
			return greetings.Any(g => message.Contains(g));
		}

		private bool IsProductSearch(string message)
		{
			var keywords = new[] { "sản phẩm", "tìm", "mua", "danh sách", "xem", "có gì", "bán gì", "show", "list" };
			return keywords.Any(k => message.Contains(k));
		}

		private bool IsOrderRelated(string message)
		{
			var keywords = new[] { "đặt hàng", "mua hàng", "thanh toán", "checkout", "cod", "vietqr", "chuyển khoản", "payment" };
			return keywords.Any(k => message.Contains(k));
		}

		private bool IsWarrantyRelated(string message)
		{
			var keywords = new[] { "bảo hành", "đổi trả", "warranty", "đổi", "trả", "lỗi", "hỏng", "sửa" };
			return keywords.Any(k => message.Contains(k));
		}

		private bool IsPriceRelated(string message)
		{
			var keywords = new[] { "giá", "price", "cost", "bao nhiêu", "rẻ", "đắt", "giảm giá", "sale" };
			return keywords.Any(k => message.Contains(k));
		}

		private bool IsShippingRelated(string message)
		{
			var keywords = new[] { "vận chuyển", "ship", "giao hàng", "delivery", "phí ship", "shipping", "giao" };
			return keywords.Any(k => message.Contains(k));
		}

		private bool IsPromotionRelated(string message)
		{
			var keywords = new[] { "khuyến mãi", "promotion", "mã giảm giá", "discount", "coupon", "ưu đãi", "sale", "giảm" };
			return keywords.Any(k => message.Contains(k));
		}

		private bool IsTechnicalSupport(string message)
		{
			var keywords = new[] { "hỗ trợ", "support", "kỹ thuật", "technical", "lắp đặt", "sử dụng", "hướng dẫn", "help" };
			return keywords.Any(k => message.Contains(k));
		}

		private bool IsContactRelated(string message)
		{
			var keywords = new[] { "liên hệ", "contact", "hotline", "số điện thoại", "phone", "email", "địa chỉ", "address" };
			return keywords.Any(k => message.Contains(k));
		}

		private string GetOrderResponse(string message)
		{
			if (message.Contains("cod") || message.Contains("tiền mặt"))
			{
				return "💵 **Thanh toán COD (Thanh toán khi nhận hàng):**\n\n" +
					"✅ Bạn có thể thanh toán bằng tiền mặt khi nhận hàng\n" +
					"✅ Không cần chuyển khoản trước\n" +
					"✅ Kiểm tra hàng trước khi thanh toán\n" +
					"✅ Phí COD: 30.000đ (miễn phí nếu đơn hàng trên 500.000đ)\n\n" +
					"Rất tiện lợi phải không ạ? 😊";
			}

			if (message.Contains("vietqr") || message.Contains("chuyển khoản") || message.Contains("qr"))
			{
				return "📱 **Thanh toán VietQR:**\n\n" +
					"✅ Quét mã QR để thanh toán nhanh chóng\n" +
					"✅ Hỗ trợ tất cả các ngân hàng\n" +
					"✅ Xác nhận thanh toán tự động\n" +
					"✅ An toàn và bảo mật\n\n" +
					"Khi đặt hàng, bạn sẽ thấy mã QR để quét thanh toán ngay!";
			}

			return "📦 **Hướng dẫn đặt hàng:**\n\n" +
				"1️⃣ **Bước 1:** Chọn sản phẩm và thêm vào giỏ hàng\n" +
				"2️⃣ **Bước 2:** Vào giỏ hàng và kiểm tra lại đơn hàng\n" +
				"3️⃣ **Bước 3:** Điền thông tin giao hàng (tên, số điện thoại, địa chỉ)\n" +
				"4️⃣ **Bước 4:** Chọn phương thức thanh toán:\n" +
				"   • 💳 VietQR (quét mã QR)\n" +
				"   • 💵 COD (thanh toán khi nhận hàng)\n" +
				"5️⃣ **Bước 5:** Xác nhận đơn hàng\n\n" +
				"🎉 Sau khi đặt hàng thành công, bạn sẽ nhận được email/SMS xác nhận!\n\n" +
				"Bạn muốn thanh toán bằng cách nào ạ?";
		}

		private async Task<string> GetPriceResponseAsync(string message)
		{
			// Nếu có từ khóa sản phẩm cụ thể, tìm sản phẩm đó
			var keywords = ExtractProductKeywords(message);
			if (keywords.Any())
			{
				var searchResult = await SearchProductsResponseAsync(keywords);
				if (!searchResult.Contains("Không tìm thấy"))
				{
					return searchResult;
				}
			}

			return "💰 **Về giá sản phẩm:**\n\n" +
				"✅ Giá được hiển thị rõ ràng trên từng trang sản phẩm\n" +
				"✅ Giá đã bao gồm VAT\n" +
				"✅ Nhiều chương trình khuyến mãi và mã giảm giá\n" +
				"✅ Giá tốt nhất thị trường, cam kết không bán đắt\n\n" +
				"💡 **Bạn muốn:**\n" +
				"• Xem giá sản phẩm cụ thể? Hãy cho tôi biết tên sản phẩm\n" +
				"• Xem sản phẩm đang giảm giá? Tôi có thể liệt kê cho bạn\n" +
				"• Nhận mã giảm giá? Đăng ký tài khoản mới nhận ngay ưu đãi 5%\n\n" +
				"Bạn đang quan tâm sản phẩm nào ạ?";
		}

		private string GetFriendlyDefaultResponse(string message, List<string> recentContext)
		{
			// Nếu có context gần đây về sản phẩm, hỏi lại
			if (recentContext.Any(c => c.Contains("sản phẩm") || c.Contains("tìm")))
			{
				return "Xin lỗi, tôi chưa hiểu rõ câu hỏi của bạn. 😅\n\n" +
					"Bạn có thể:\n" +
					"• Hỏi lại cụ thể hơn về sản phẩm\n" +
					"• Cho tôi biết bạn đang tìm sản phẩm gì\n" +
					"• Hoặc gọi hotline 1900 1234 để được tư vấn trực tiếp\n\n" +
					"Tôi sẵn sàng giúp đỡ bạn! 😊";
			}

			var responses = new[]
			{
				"Cảm ơn bạn đã liên hệ! 😊 Tôi có thể giúp bạn:\n\n" +
				"🛍️ Tìm kiếm và tư vấn sản phẩm\n" +
				"📦 Hướng dẫn đặt hàng và thanh toán\n" +
				"📋 Theo dõi đơn hàng\n" +
				"🔄 Chính sách bảo hành và đổi trả\n" +
				"🚚 Thông tin vận chuyển\n" +
				"🎁 Chương trình khuyến mãi\n" +
				"🔧 Hỗ trợ kỹ thuật\n\n" +
				"Bạn cần hỗ trợ gì cụ thể ạ?",

				"Xin chào! 👋 Tôi là nhân viên tư vấn của Điện Nhà. Tôi có thể:\n\n" +
				"• Tư vấn sản phẩm phù hợp với nhu cầu\n" +
				"• Hướng dẫn đặt hàng chi tiết\n" +
				"• Giải đáp mọi thắc mắc về sản phẩm và dịch vụ\n" +
				"• Hỗ trợ kỹ thuật và bảo hành\n\n" +
				"Bạn đang quan tâm điều gì ạ?",

				"Rất vui được hỗ trợ bạn! 😊\n\n" +
				"Tôi có thể giúp bạn tìm sản phẩm, đặt hàng, hoặc giải đáp bất kỳ câu hỏi nào. " +
				"Bạn cũng có thể gọi hotline 1900 1234 (8:00-22:00) để được tư vấn trực tiếp.\n\n" +
				"Bạn cần hỗ trợ gì ạ?"
			};

			return responses[new Random().Next(responses.Length)];
		}

		private async Task<string> GetProductsResponseAsync(string message)
		{
			try
			{
				var products = await _context.Products
					.Include(p => p.Category)
					.Where(p => p.IsActive)
					.OrderByDescending(p => p.CreatedAt)
					.Take(6)
					.ToListAsync();

				if (!products.Any())
				{
					return "Hiện tại chúng tôi chưa có sản phẩm nào. Vui lòng quay lại sau!";
				}

				var response = new StringBuilder();
				response.AppendLine("📦 **Danh sách sản phẩm nổi bật:**\n");

				foreach (var product in products)
				{
					var price = product.Price.ToString("N0");
					var stockStatus = product.Stock > 0 ? "✅ Còn hàng" : "❌ Hết hàng";
					response.AppendLine($"• **{product.Name}**");
					response.AppendLine($"  💰 Giá: {price} VNĐ");
					response.AppendLine($"  📂 Danh mục: {product.Category?.Name ?? "N/A"}");
					response.AppendLine($"  {stockStatus}");
					response.AppendLine($"  🔗 Xem chi tiết: /Product/Details/{product.Id}");
					response.AppendLine();
				}

				response.AppendLine("💡 Bạn có thể xem tất cả sản phẩm tại: /Product");
				response.AppendLine("🔍 Hoặc tìm kiếm sản phẩm cụ thể bằng cách nhập tên sản phẩm.");

				return response.ToString();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting products");
				return "Xin lỗi, tôi không thể lấy danh sách sản phẩm lúc này. Vui lòng thử lại sau.";
			}
		}

		private async Task<string> SearchProductsResponseAsync(List<string> keywords)
		{
			try
			{
				var query = _context.Products
					.Include(p => p.Category)
					.Where(p => p.IsActive);

				// Tìm kiếm theo từ khóa
				var searchTerm = string.Join(" ", keywords);
				query = query.Where(p => 
					p.Name.Contains(searchTerm) || 
					p.Description != null && p.Description.Contains(searchTerm) ||
					p.Category != null && p.Category.Name.Contains(searchTerm));

				var products = await query
					.OrderByDescending(p => p.CreatedAt)
					.Take(5)
					.ToListAsync();

				if (!products.Any())
				{
					return $"Không tìm thấy sản phẩm nào với từ khóa \"{searchTerm}\".\n\n" +
						"💡 Bạn có thể:\n" +
						"• Xem tất cả sản phẩm: /Product\n" +
						"• Thử tìm kiếm với từ khóa khác\n" +
						"• Xem danh mục sản phẩm";
				}

				var response = new StringBuilder();
				response.AppendLine($"🔍 **Tìm thấy {products.Count} sản phẩm:**\n");

				foreach (var product in products)
				{
					var price = product.Price.ToString("N0");
					var stockStatus = product.Stock > 0 ? "✅ Còn hàng" : "❌ Hết hàng";
					response.AppendLine($"• **{product.Name}**");
					response.AppendLine($"  💰 Giá: {price} VNĐ");
					response.AppendLine($"  📂 Danh mục: {product.Category?.Name ?? "N/A"}");
					response.AppendLine($"  {stockStatus}");
					response.AppendLine($"  🔗 Xem chi tiết: /Product/Details/{product.Id}");
					response.AppendLine();
				}

				return response.ToString();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error searching products");
				return "Xin lỗi, có lỗi xảy ra khi tìm kiếm. Vui lòng thử lại sau.";
			}
		}

		private async Task<string> GetCategoriesResponseAsync()
		{
			try
			{
				var categories = await _context.Categories
					.Where(c => c.IsActive)
					.OrderBy(c => c.Name)
					.ToListAsync();

				if (!categories.Any())
				{
					return "Hiện tại chưa có danh mục sản phẩm nào.";
				}

				var response = new StringBuilder();
				response.AppendLine("📂 **Danh mục sản phẩm:**\n");

				foreach (var category in categories)
				{
					var productCount = await _context.Products
						.CountAsync(p => p.CategoryId == category.Id && p.IsActive);
					
					response.AppendLine($"• **{category.Name}** ({productCount} sản phẩm)");
					if (!string.IsNullOrEmpty(category.Description))
					{
						response.AppendLine($"  {category.Description}");
					}
					response.AppendLine();
				}

				response.AppendLine("💡 Bạn có thể xem sản phẩm theo danh mục tại: /Product");

				return response.ToString();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting categories");
				return "Xin lỗi, không thể lấy danh sách danh mục. Vui lòng thử lại sau.";
			}
		}

		private List<string> ExtractProductKeywords(string message)
		{
			// Loại bỏ các từ không cần thiết
			var stopWords = new[] { "tìm", "có", "bán", "sản phẩm", "mua", "xem", "cho", "tôi", "bạn", "của", "và", "hoặc" };
			var words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.Where(w => !stopWords.Contains(w.ToLower()))
				.Where(w => w.Length > 2) // Chỉ lấy từ có độ dài > 2
				.ToList();
			
			return words;
		}

		private async Task<string> GenerateAIResponseAsync(string userMessage, ChatConversation conversation)
		{
			var aiProvider = _configuration["AI:Provider"] ?? "OpenAI";
			var apiKey = _configuration["AI:ApiKey"];

			if (string.IsNullOrEmpty(apiKey))
			{
				return await GenerateRuleBasedResponseAsync(userMessage.ToLower());
			}

			try
			{
				// Lấy lịch sử tin nhắn gần đây
				var recentMessages = await _context.ChatMessages
					.Where(m => m.ConversationId == conversation.Id)
					.OrderByDescending(m => m.SentAt)
					.Take(10)
					.OrderBy(m => m.SentAt)
					.ToListAsync();

				// Tạo context từ lịch sử
				var context = new StringBuilder();
				context.AppendLine("Bạn là trợ lý AI của cửa hàng Điện Nhà - chuyên bán thiết bị điện gia dụng.");
				context.AppendLine("Nhiệm vụ: Hỗ trợ khách hàng tìm sản phẩm, đặt hàng, theo dõi đơn hàng, và giải đáp thắc mắc.");
				context.AppendLine("Hãy trả lời ngắn gọn, thân thiện, bằng tiếng Việt.");

				foreach (var msg in recentMessages)
				{
					context.AppendLine($"{msg.Sender}: {msg.Content}");
				}

				if (aiProvider == "OpenAI")
				{
					return await CallOpenAIAsync(apiKey, userMessage, context.ToString());
				}
				else if (aiProvider == "AzureOpenAI")
				{
					return await CallAzureOpenAIAsync(userMessage, context.ToString());
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error calling AI service");
			}

			return await GenerateRuleBasedResponseAsync(userMessage.ToLower());
		}

		private async Task<string> CallOpenAIAsync(string apiKey, string userMessage, string context)
		{
			var client = _httpClientFactory.CreateClient();
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

			var payload = new
			{
				model = _configuration["AI:Model"] ?? "gpt-3.5-turbo",
				messages = new[]
				{
					new { role = "system", content = context },
					new { role = "user", content = userMessage }
				},
				max_tokens = 500,
				temperature = 0.7
			};

			var json = JsonSerializer.Serialize(payload);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
			if (response.IsSuccessStatusCode)
			{
				var responseContent = await response.Content.ReadAsStringAsync();
				var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
				if (result.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
				{
					var firstChoice = choices[0];
					if (firstChoice.TryGetProperty("message", out var message))
					{
						if (message.TryGetProperty("content", out var contentProp))
						{
							return contentProp.GetString() ?? await GenerateRuleBasedResponseAsync(userMessage.ToLower());
						}
					}
				}
			}

			return await GenerateRuleBasedResponseAsync(userMessage.ToLower());
		}

		private async Task<string> CallAzureOpenAIAsync(string userMessage, string context)
		{
			var endpoint = _configuration["AI:Endpoint"];
			var apiKey = _configuration["AI:ApiKey"];
			var deployment = _configuration["AI:Deployment"] ?? "gpt-35-turbo";

			if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
			{
				return await GenerateRuleBasedResponseAsync(userMessage.ToLower());
			}

			var client = _httpClientFactory.CreateClient();
			client.DefaultRequestHeaders.Add("api-key", apiKey);

			var payload = new
			{
				messages = new[]
				{
					new { role = "system", content = context },
					new { role = "user", content = userMessage }
				},
				max_tokens = 500,
				temperature = 0.7
			};

			var json = JsonSerializer.Serialize(payload);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			var url = $"{endpoint}/openai/deployments/{deployment}/chat/completions?api-version=2024-02-15-preview";
			var response = await client.PostAsync(url, content);

			if (response.IsSuccessStatusCode)
			{
				var responseContent = await response.Content.ReadAsStringAsync();
				var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
				if (result.TryGetProperty("choices", out var choices) && choices.GetArrayLength() > 0)
				{
					var firstChoice = choices[0];
					if (firstChoice.TryGetProperty("message", out var message))
					{
						if (message.TryGetProperty("content", out var contentProp))
						{
							return contentProp.GetString() ?? await GenerateRuleBasedResponseAsync(userMessage.ToLower());
						}
					}
				}
			}

			return await GenerateRuleBasedResponseAsync(userMessage.ToLower());
		}

		public async Task<List<ChatMessage>> GetConversationMessagesAsync(int conversationId)
		{
			return await _context.ChatMessages
				.Where(m => m.ConversationId == conversationId)
				.OrderBy(m => m.SentAt)
				.ToListAsync();
		}

		public async Task<List<ChatConversation>> GetUserConversationsAsync(string? userId)
		{
			if (string.IsNullOrEmpty(userId))
				return new List<ChatConversation>();

			return await _context.ChatConversations
				.Where(c => c.UserId == userId)
				.OrderByDescending(c => c.StartedAt)
				.Include(c => c.Messages.OrderBy(m => m.SentAt).Take(1))
				.ToListAsync();
		}
	}
}

