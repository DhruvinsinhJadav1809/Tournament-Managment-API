using GamesAPI.Api.Models;

namespace GamesAPI.Api.DTOs.Games
{
    public class GetGamesResponse
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public List<Game> Data { get; set; }
            = new();
    }
}