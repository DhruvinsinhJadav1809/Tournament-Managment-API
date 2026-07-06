
public interface IOnlineUserService
{
    Task<OnlineUsersResponse> GetOnlineUsersAsync();
    Task<int> GetOnlineUserCountAsync();
}