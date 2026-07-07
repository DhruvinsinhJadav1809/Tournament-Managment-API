using GamesAPI.Api.Data;
using GamesAPI.Api.Exceptions;
using GamesAPI.Api.Interfaces;
using GamesAPI.Api.Models.Chat.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using GamesAPI.Api.Hubs;
using GamesAPI.Api.Services.Security;
namespace GamesAPI.Api.Services
{
    public class ConversationService : IConversationService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ICryptoService _cryptoService;
        public ConversationService(
            AppDbContext context,
             IHubContext<ChatHub> hubContext,
              ICryptoService cryptoService)
        {
            _context = context;
            _hubContext = hubContext;
            _cryptoService = cryptoService;
        }

        public async Task<ConversationResponse> GetOrCreateConversationAsync(
            Guid currentUserId,
            GetOrCreateConversationRequest request)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c =>
                c.Type == ConversationType.Private &&
                c.Participants.Count == 2 &&
                c.Participants.Any(p => p.UserId == currentUserId) &&
                c.Participants.Any(p => p.UserId == request.TargetUserId));

            if (conversation != null)
            {
                return new ConversationResponse
                {
                    ConversationId = conversation.Id,
                    IsNewConversation = false
                };
            }

            conversation = new Conversation
            {
                Id = Guid.NewGuid(),

                Type = ConversationType.Private,

                CreatedByUserId = currentUserId,

                CreatedAt = DateTime.Now,

                IsActive = true,

                IsDeleted = false
            };

            var currentParticipant = new ConversationParticipant
            {
                Id = Guid.NewGuid(),

                ConversationId = conversation.Id,

                UserId = currentUserId,

                JoinedAt = DateTime.Now
            };

            var targetParticipant = new ConversationParticipant
            {
                Id = Guid.NewGuid(),

                ConversationId = conversation.Id,

                UserId = request.TargetUserId,

                JoinedAt = DateTime.Now
            };
            _context.Conversations.Add(conversation);

            _context.ConversationParticipants.Add(currentParticipant);

            _context.ConversationParticipants.Add(targetParticipant);
            await _context.SaveChangesAsync();
            return new ConversationResponse
            {
                ConversationId = conversation.Id,
                IsNewConversation = true
            };
        }

        public async Task<MessageResponse> SendMessageAsync(
                 Guid currentUserId,
                 SendMessageRequest request)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c =>
                    c.Id == request.ConversationId);
            if (conversation == null)
            {
                throw new ApiException(
                    "Conversation not found.",
                    StatusCodes.Status404NotFound);
            }
            var isParticipant = conversation.Participants
                 .Any(x => x.UserId == currentUserId);

            if (!isParticipant)
            {
                throw new ApiException(
                    "You are not a participant of this conversation.",
                    StatusCodes.Status403Forbidden);
            }
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                throw new ApiException(
                    "Message cannot be empty.",
                    StatusCodes.Status400BadRequest);
            }
            var message = new ConversationMessage
            {
                Id = Guid.NewGuid(),

                ConversationId = conversation.Id,

                SenderId = currentUserId,

                Message = _cryptoService.Encrypt(request.Message.Trim()),

                SentAt = DateTime.Now,

                IsEdited = false,

                IsDeleted = false
            };
            _context.ConversationMessages.Add(message);

            await _context.SaveChangesAsync();

            var sender = await _context.Users
            .FirstAsync(x => x.Id == currentUserId);

            var response = new MessageResponse
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = sender.FullName,
                Message = _cryptoService.Decrypt(message.Message),
                SentAt = message.SentAt,
                IsEdited = message.IsEdited
            };

            await _hubContext.Clients
                .Group($"conversation-{message.ConversationId}")
                .SendAsync("ReceiveMessage", response);

            return response;
        }

        public async Task<List<MessageResponse>> GetMessagesAsync(
            Guid currentUserId,
            Guid conversationId)
        {
            var conversation = await _context.Conversations
            .Include(c => c.Participants)
            .FirstOrDefaultAsync(c =>
                c.Id == conversationId);
            if (conversation == null)
            {
                throw new ApiException(
                    "Conversation not found.",
                    StatusCodes.Status404NotFound);
            }
            var isParticipant = conversation.Participants
             .Any(x => x.UserId == currentUserId);

            if (!isParticipant)
            {
                throw new ApiException(
                    "You are not a participant of this conversation.",
                    StatusCodes.Status403Forbidden);
            }
            var messages = await _context.ConversationMessages
            .Where(m =>
                m.ConversationId == conversationId &&
                !m.IsDeleted)
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageResponse
            {
                Id = m.Id,

                ConversationId = m.ConversationId,

                SenderId = m.SenderId,

                SenderName = m.Sender.FullName,

                Message = _cryptoService.Decrypt(m.Message),

                SentAt = m.SentAt,

                IsEdited = m.IsEdited
            })
            .ToListAsync();
            return messages;
        }

        public async Task MarkConversationAsReadAsync(
                    Guid currentUserId,
                    MarkConversationAsReadRequest request)
        {
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(x =>
                    x.ConversationId == request.ConversationId &&
                    x.UserId == currentUserId);
            if (participant == null)
            {
                throw new ApiException(
                    "Conversation not found.",
                    StatusCodes.Status404NotFound);
            }
            participant.LastSeenMessageId =
                request.LastSeenMessageId;
            await _context.SaveChangesAsync();
        }
    }
}