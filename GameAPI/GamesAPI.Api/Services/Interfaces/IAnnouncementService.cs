using GamesAPI.Api.DTOs;

public interface IAnnouncementService
{
    Task CreateAnnouncementAsync(
        Guid adminId,
        CreateAnnouncementRequest request);
    Task<List<AnnouncementResponse>>
    GetUserAnnouncementsAsync(
    Guid userId);
}