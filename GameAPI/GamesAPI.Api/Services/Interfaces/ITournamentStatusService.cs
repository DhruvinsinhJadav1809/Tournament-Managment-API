using GamesAPI.Api.Models;
using GamesAPI.Api.DTOs.TournamentStatuses;

namespace GamesAPI.Api.Interfaces
{
    public interface ITournamentStatusService
    {
        Task<List<TournamentStatus>> GetAllAsync();

        Task<TournamentStatus?> GetByIdAsync(Guid id);

        Task<TournamentStatus> CreateAsync(CreateTournamentStatusRequest request);

        Task<TournamentStatus?> UpdateAsync(Guid id, UpdateTournamentStatusRequest request);

        Task<bool> DeleteAsync(Guid id);
    }
}
