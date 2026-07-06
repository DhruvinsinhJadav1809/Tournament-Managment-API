using GamesAPI.Api.DTOs.Games;
using GamesAPI.Api.Models;

namespace GamesAPI.Api.Interfaces
{
    public interface IGameService
    {
        Task<GetGamesResponse> GetGamesAsync(
            GetGamesRequest request
            );

        Task<Game?> GetGameByIdAsync(
            Guid id);

        Task<Game> CreateGameAsync(
            CreateGameRequest request,
            Guid userId);

        Task<Game?> UpdateGameAsync(
            Guid id,
            UpdateGameRequest request,
            Guid userId);

        Task<bool> DeleteGameAsync(
            Guid id,
            Guid userId);
        Task<List<Game>> GetAllGames();
    }
}