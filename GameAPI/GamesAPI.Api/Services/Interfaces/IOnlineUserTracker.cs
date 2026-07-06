using GamesAPI.Application.Services.OnlineUsers.Models;

public interface IOnlineUserTracker
{
    //only updates the memory not the sql that why void
    void AddUser(
        Guid userId,
        string userName,
        string connectionId);

    void RemoveConnection(
        string connectionId);

    IReadOnlyCollection<OnlineUser> GetOnlineUsers();

    int GetOnlineUserCount();
}