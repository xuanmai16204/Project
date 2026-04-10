$(document).ready(function() {
	let conversationId = null;
	let isMinimized = false;

	// Toggle chatbot
	$('#chatbot-toggle').on('click', function() {
		if (isMinimized) {
			$('#chatbot-container').slideDown();
			isMinimized = false;
		} else {
			$('#chatbot-container').slideToggle();
		}
		$('.chatbot-badge').hide();
	});

	// Minimize chatbot
	$('#chatbot-minimize').on('click', function() {
		$('#chatbot-container').slideUp();
		isMinimized = true;
	});

	// Close chatbot
	$('#chatbot-close').on('click', function() {
		$('#chatbot-container').slideUp();
		isMinimized = true;
	});

	// Send message
	function sendMessage() {
		const input = $('#chatbot-input');
		const message = input.val().trim();

		if (!message) return;

		// Add user message to chat
		addMessage(message, 'user');

		// Clear input
		input.val('');

		// Show typing indicator
		showTypingIndicator();

		// Send to server
		$.ajax({
			url: '/Chatbot/SendMessage',
			method: 'POST',
			contentType: 'application/json',
			data: JSON.stringify({
				message: message,
				conversationId: conversationId
			}),
			success: function(response) {
				hideTypingIndicator();
				if (response.success) {
					conversationId = response.conversationId || conversationId;
					addMessage(response.message, 'bot');
				} else {
					addMessage('Xin lỗi, có lỗi xảy ra. Vui lòng thử lại sau.', 'bot');
				}
			},
			error: function() {
				hideTypingIndicator();
				addMessage('Xin lỗi, không thể kết nối đến server. Vui lòng thử lại sau.', 'bot');
			}
		});
	}

	// Add message to chat
	function addMessage(content, sender) {
		const messagesContainer = $('#chatbot-messages');
		const messageClass = sender === 'user' ? 'user-message' : 'bot-message';
		const avatarIcon = sender === 'user' ? 'bi-person-fill' : 'bi-robot';

		// Format content: convert markdown-like syntax to HTML
		const formattedContent = formatMessageContent(content);

		const messageHtml = `
			<div class="chatbot-message ${messageClass}">
				<div class="message-avatar">
					<i class="bi ${avatarIcon}"></i>
				</div>
				<div class="message-content">
					${formattedContent}
				</div>
			</div>
		`;

		messagesContainer.append(messageHtml);
		scrollToBottom();
	}

	// Format message content with links and formatting
	function formatMessageContent(content) {
		// Escape HTML first
		let formatted = escapeHtml(content);
		
		// Convert links /Product/Details/123 to clickable links (before other processing)
		formatted = formatted.replace(/🔗\s*Xem chi tiết:\s*\/Product\/Details\/(\d+)/g, 
			'<a href="/Product/Details/$1" class="chatbot-link" target="_blank">🔗 Xem chi tiết sản phẩm</a>');
		
		// Convert links /Product to clickable links
		formatted = formatted.replace(/💡\s*Bạn có thể xem tất cả sản phẩm tại:\s*\/Product/g, 
			'<a href="/Product" class="chatbot-link" target="_blank">💡 Xem tất cả sản phẩm</a>');
		
		// Convert bold text **text** to <strong>text</strong>
		formatted = formatted.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');
		
		// Convert emoji codes to actual display
		formatted = formatted.replace(/📦/g, '📦');
		formatted = formatted.replace(/💰/g, '💰');
		formatted = formatted.replace(/📂/g, '📂');
		formatted = formatted.replace(/✅/g, '✅');
		formatted = formatted.replace(/❌/g, '❌');
		formatted = formatted.replace(/🔍/g, '🔍');
		formatted = formatted.replace(/💡/g, '💡');
		
		// Convert line breaks
		formatted = formatted.split('\n').map(line => {
			// Convert bullet points • to styled list
			if (line.trim().startsWith('•')) {
				return `<div class="chatbot-list-item">${line.trim()}</div>`;
			}
			// Convert numbered list
			if (/^\d+\.\s/.test(line.trim())) {
				return `<div class="chatbot-list-item">${line.trim()}</div>`;
			}
			return line;
		}).join('<br>');
		
		// Wrap in paragraph
		return `<p>${formatted}</p>`;
	}

	// Show typing indicator
	function showTypingIndicator() {
		const messagesContainer = $('#chatbot-messages');
		const typingHtml = `
			<div class="chatbot-message bot-message" id="typing-indicator">
				<div class="message-avatar">
					<i class="bi bi-robot"></i>
				</div>
				<div class="message-content">
					<div class="typing-indicator">
						<span></span>
						<span></span>
						<span></span>
					</div>
				</div>
			</div>
		`;
		messagesContainer.append(typingHtml);
		scrollToBottom();
	}

	// Hide typing indicator
	function hideTypingIndicator() {
		$('#typing-indicator').remove();
	}

	// Scroll to bottom
	function scrollToBottom() {
		const messagesContainer = $('#chatbot-messages');
		messagesContainer.scrollTop(messagesContainer[0].scrollHeight);
	}

	// Escape HTML
	function escapeHtml(text) {
		const map = {
			'&': '&amp;',
			'<': '&lt;',
			'>': '&gt;',
			'"': '&quot;',
			"'": '&#039;'
		};
		return text.replace(/[&<>"']/g, m => map[m]);
	}

	// Send button click
	$('#chatbot-send').on('click', sendMessage);

	// Enter key press
	$('#chatbot-input').on('keypress', function(e) {
		if (e.which === 13) {
			e.preventDefault();
			sendMessage();
		}
	});

	// Load conversation history if conversationId exists
	if (conversationId) {
		loadConversation(conversationId);
	}

	function loadConversation(id) {
		$.ajax({
			url: `/Chatbot/GetMessages?conversationId=${id}`,
			method: 'GET',
			success: function(response) {
				if (response.success && response.messages) {
					const messagesContainer = $('#chatbot-messages');
					messagesContainer.empty();
					
					response.messages.forEach(function(msg) {
						addMessage(msg.content, msg.isFromBot ? 'bot' : 'user');
					});
				}
			}
		});
	}
});

