namespace GamesAPI.Api.DTOs.Games
{
    public class GetGamesRequest
    {
        public string? Search { get; set; }

        public bool? IsActive { get; set; }

        public int? ParticipantsPerMatch { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}