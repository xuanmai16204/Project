using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebsiteDienNha_DoAnChuyenNganh.Models;
using WebsiteDienNha_DoAnChuyenNganh.Services;

namespace WebsiteDienNha_DoAnChuyenNganh.Controllers
{
	public class ChatbotController : Controller
	{
		private readonly ChatbotService _chatbotService;
		private readonly UserManager<ApplicationUser> _userManager;

		public ChatbotController(ChatbotService chatbotService, UserManager<ApplicationUser> userManager)
		{
			_chatbotService = chatbotService;
			_userManager = userManager;
		}

		[HttpPost]
		public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
		{
			try
			{
				var userId = _userManager.GetUserId(User);
				var response = await _chatbotService.GetResponseAsync(
					request.Message,
					userId,
					request.ConversationId
				);

				// Lấy conversation ID mới nhất
				var conversations = await _chatbotService.GetUserConversationsAsync(userId);
				var currentConversation = conversations.FirstOrDefault();

				return Json(new
				{
					success = true,
					message = response,
					conversationId = currentConversation?.Id ?? request.ConversationId
				});
			}
			catch (Exception ex)
			{
				return Json(new
				{
					success = false,
					message = "Có lỗi xảy ra. Vui lòng thử lại sau."
				});
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetMessages(int conversationId)
		{
			try
			{
				var messages = await _chatbotService.GetConversationMessagesAsync(conversationId);
				return Json(new { success = true, messages });
			}
			catch
			{
				return Json(new { success = false, messages = new List<ChatMessage>() });
			}
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetConversations()
		{
			try
			{
				var userId = _userManager.GetUserId(User);
				var conversations = await _chatbotService.GetUserConversationsAsync(userId);
				return Json(new { success = true, conversations });
			}
			catch
			{
				return Json(new { success = false, conversations = new List<ChatConversation>() });
			}
		}
	}

	public class ChatRequest
	{
		public string Message { get; set; } = string.Empty;
		public int? ConversationId { get; set; }
	}
}

