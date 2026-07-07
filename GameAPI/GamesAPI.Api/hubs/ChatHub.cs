using Microsoft.AspNetCore.SignalR;

namespace GamesAPI.Api.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinConversation(
            Guid conversationId)
        {
            Console.WriteLine($"Joining conversation group: conversation-{conversationId}");

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"conversation-{conversationId}");
        }

        public async Task LeaveConversation(
            Guid conversationId)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"conversation-{conversationId}");
        }
    }
}