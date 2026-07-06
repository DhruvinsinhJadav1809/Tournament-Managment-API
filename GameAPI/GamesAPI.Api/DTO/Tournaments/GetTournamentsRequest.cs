namespace GamesAPI.Api.DTOs.Tournaments
{
    public class GetTournamentsRequest
    {
        public string? Search { get; set; }

        public bool? IsActive { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public Guid? GameId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
    }
}