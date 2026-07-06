using GamesAPI.Api.Models;

public class GetUsersResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime? CreatedAt
    {
        get;
        set;
    }
}