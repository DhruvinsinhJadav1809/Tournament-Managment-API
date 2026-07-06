namespace GamesAPI.Application.Services.OnlineUsers.Models;

public class OnlineUser
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public DateTime ConnectedAt { get; set; }

    public HashSet<string> ConnectionIds { get; set; } = new();
}