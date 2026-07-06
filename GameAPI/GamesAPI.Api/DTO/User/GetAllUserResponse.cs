using GamesAPI.Api.Models;

public class GetAllUserResponse
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }
    public List<GetUsersResponse> Data { get; set; } = new();

}