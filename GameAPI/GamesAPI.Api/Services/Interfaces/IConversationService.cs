namespace GamesAPI.Api.Interfaces
{
    public interface IConversationService
    {
        Task<ConversationResponse> GetOrCreateConversationAsync(
            Guid currentUserId,
            GetOrCreateConversationRequest request);

        Task<MessageResponse> SendMessageAsync(
            Guid currentUserId,
            SendMessageRequest request);

        Task<List<MessageResponse>> GetMessagesAsync(
            Guid currentUserId,
            Guid conversationId);

        Task MarkConversationAsReadAsync(
            Guid currentUserId,
            MarkConversationAsReadRequest request);
    }
}