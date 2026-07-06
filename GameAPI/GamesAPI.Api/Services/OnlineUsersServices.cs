


public class OnlineUserService
    : IOnlineUserService
{
    private readonly IOnlineUserTracker _onlineUserTracker;

    public OnlineUserService(
        IOnlineUserTracker onlineUserTracker)
    {
        _onlineUserTracker = onlineUserTracker;
    }

    public Task<OnlineUsersResponse> GetOnlineUsersAsync()
    {
        var users = _onlineUserTracker
            .GetOnlineUsers()
            .Select(x => new OnlineUserResponse
            {
                UserId = x.UserId,
                UserName = x.UserName,
                ConnectedAt = x.ConnectedAt
            })
            .OrderBy(x => x.UserName)
            .ToList();

        var response = new OnlineUsersResponse
        {
            OnlineUserCount = users.Count,
            Users = users
        };

        return Task.FromResult(response);
    }



    public Task<int> GetOnlineUserCountAsync()
    {
        return Task.FromResult(_onlineUserTracker.GetOnlineUserCount());
    }
}
