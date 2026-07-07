using GamesAPI.Api.DTOs;
using GamesAPI.Api.Extensions;
using GamesAPI.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesAPI.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        public ConversationController(
            IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        /// <summary>
        /// Starting of the conversation
        /// </summary>
        /// <param name="conversationService"></param>
        [HttpPost("get-or-create")]
        public async Task<IActionResult> GetOrCreateConversation(
        GetOrCreateConversationRequest request)
        {
            var currentUserId = this.GetUserId();

            var response = await _conversationService
                .GetOrCreateConversationAsync(
                    currentUserId,
                    request);
            return Ok(new ApiResponse<ConversationResponse>
            {
                Success = true,
                Message = "Conversation retrieved successfully.",
                Data = response
            });
        }
        /// <summary>
        /// Send messages for the chat
        /// </summary>
        /// <param name="request"></param>

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage(
    SendMessageRequest request)
        {
            var currentUserId = this.GetUserId();

            var response = await _conversationService
                .SendMessageAsync(
                    currentUserId,
                    request);

            return Ok(new ApiResponse<MessageResponse>
            {
                Success = true,
                Message = "Conversation retrieved successfully.",
                Data = response
            });
        }

        /// <summary>
        /// Get messages for the conversation by id
        /// </summary>

        [HttpGet("{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(Guid conversationId)
        {
            var currentUserId = this.GetUserId();

            var response = await _conversationService
                .GetMessagesAsync(
                    currentUserId,
                    conversationId);

            return Ok(new ApiResponse<List<MessageResponse>>
            {
                Success = true,
                Message = "Messages retrieved successfully.",
                Data = response
            });
        }
        /// <summary>
        /// Mark message as read
        /// </summary>

        [HttpPut("mark-as-read")]
        public async Task<IActionResult> MarkAsRead(
    MarkConversationAsReadRequest request)
        {
            var currentUserId = this.GetUserId();

            await _conversationService.MarkConversationAsReadAsync(
                currentUserId,
                request);

            return Ok();
        }
    }
}