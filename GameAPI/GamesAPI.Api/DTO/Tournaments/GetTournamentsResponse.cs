using GamesAPI.Api.Models;

namespace GamesAPI.Api.DTOs.Tournaments
{
    public class GetTournamentsResponse
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public List<TournamentResponse> Data { get; set; }
            = new();
    }
}