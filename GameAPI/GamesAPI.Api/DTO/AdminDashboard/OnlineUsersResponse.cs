public class OnlineUsersResponse
{
    public int OnlineUserCount { get; set; }

    public List<OnlineUserResponse> Users { get; set; } = [];
}