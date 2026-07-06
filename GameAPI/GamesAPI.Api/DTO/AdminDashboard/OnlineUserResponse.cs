public class OnlineUserResponse
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public DateTime ConnectedAt { get; set; }
}