using GamesAPI.Application.Services.OnlineUsers.Models;

public class OnlineUserTracker : IOnlineUserTracker
{
    private readonly Dictionary<Guid, OnlineUser> _onlineUsers = new();

    public void AddUser(
        Guid userId,
        string userName,
        string connectionId)
    {

        // Check if user is already online
        if (_onlineUsers.TryGetValue(userId, out var onlineUser))
        {
            // User already exists
            // Add another browser/device connection
            onlineUser.ConnectionIds.Add(connectionId);

            return;
        }

        // First connection of this user
        _onlineUsers[userId] = new OnlineUser
        {
            UserId = userId,
            UserName = userName,
            ConnectedAt = DateTime.UtcNow,
            ConnectionIds = new HashSet<string>
            {
                connectionId
            }

        };

    }

    public void RemoveConnection(string connectionId)
    {
        // Find which user owns this connection
        var onlineUser = _onlineUsers.Values.FirstOrDefault(x =>
            x.ConnectionIds.Contains(connectionId));

        if (onlineUser == null)
        {
            return;
        }

        // Remove the closed connection
        onlineUser.ConnectionIds.Remove(connectionId);

        // If no connections remain,
        // user is completely offline
        if (onlineUser.ConnectionIds.Count == 0)
        {
            _onlineUsers.Remove(onlineUser.UserId);
        }
    }

    public IReadOnlyCollection<OnlineUser> GetOnlineUsers()
    {


        return _onlineUsers.Values.ToList().AsReadOnly();
    }

    public int GetOnlineUserCount()
    {
        return _onlineUsers.Count;
    }

}