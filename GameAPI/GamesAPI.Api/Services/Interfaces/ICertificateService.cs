using GamesAPI.Api.DTOs;

namespace GamesAPI.Api.Interfaces;

public interface ICertificateService
{
    Task<byte[]> GenerateWinnerCertificateAsync(
        WinnerCertificateRequest request);
    Task SendWinnerCertificateAsync(Guid tournamentId);
}