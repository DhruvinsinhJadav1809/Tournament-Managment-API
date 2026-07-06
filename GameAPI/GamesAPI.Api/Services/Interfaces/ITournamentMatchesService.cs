using Microsoft.AspNetCore.Mvc;
namespace GamesAPI.Api.Interfaces
{
    public interface ITournamentMatchesService
    {
        Task
        GenerateMatchesAsync(
        Guid tournamentId,
        GenerateMatchesRequest request);

        Task<TournamentMatchesResponse> GetTournamentMatchesAsync(Guid tournamentId);
        Task UpdateMatchResultAsync(Guid matchId,
                UpdateMatchResultRequest request);
        Task UpdateMatchScheduleAsync(
    Guid matchId,
    UpdateMatchScheduleRequest request);
        Task<List<UpcomingMatchResponse>> GetUpcomingMatchesAsync(Guid userId);
    }
}