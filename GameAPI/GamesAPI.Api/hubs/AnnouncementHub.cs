using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace GamesAPI.Api.Hubs
{
    public class AnnouncementHub : Hub
    {
        private readonly IOnlineUserTracker _onlineUserTracker;

        public AnnouncementHub(
            IOnlineUserTracker onlineUserTracker)
        {
            _onlineUserTracker = onlineUserTracker;
        }
        public async Task JoinUserGroup(
            string userId)
        {

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                userId);
        }
        public override async Task OnConnectedAsync()
        {

            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                await base.OnConnectedAsync();
                return;
            }


            var userName = Context.User?.Identity?.Name ?? "Unknown";

            _onlineUserTracker.AddUser(
                userId,
                userName,
                Context.ConnectionId);


            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(
    Exception? exception)
        {

            _onlineUserTracker.RemoveConnection(
                Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);
        }
    }
}

