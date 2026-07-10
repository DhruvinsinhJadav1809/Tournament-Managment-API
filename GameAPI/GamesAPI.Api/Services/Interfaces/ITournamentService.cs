using GamesAPI.Api.DTOs.Tournaments;
using GamesAPI.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace GamesAPI.Api.Interfaces
{
    public interface ITournamentService
    {
        Task<GetTournamentsResponse> GetTournamentsAsync(
            GetTournamentsRequest request);

        Task<TournamentResponse> GetTournamentByIdAsync(
            Guid id);

        Task<Tournament> CreateTournamentAsync(
            CreateTournamentRequest request, Guid userId);

        Task<Tournament?> UpdateTournamentAsync(
            Guid id,
            UpdateTournamentRequest request,
            Guid updateByUserId);

        Task<bool> DeleteTournamentAsync(
            Guid id, Guid userId);

        Task JoinTournamentAsync(
            Guid tournamentId,
            Guid userId);
        Task<List<TournamentDashboardResponse>>
            GetDashboardTournamentsAsync(Guid userId);
        Task<TournamentParticipantsResponse>
        GetTournamentParticipantsAsync(
            Guid tournamentId);
        Task<GetMyTournamentsResponse> GetMyTournamentsAsync(
                        Guid userId);
        Task<byte[]> DownloadWinnerCertificateAsync(
        Guid tournamentId);
    }
}