using GamesAPI.Api.DTOs.Games;
using GamesAPI.Api.Models;
namespace GamesAPI.Api.Interfaces;

public interface IInvitationService
{
    Task InviteUserAsync(
        Guid currentUserId,
        InviteUserRequest request);
    Task<InvitationDetailsResponse> GetInvitationAsync(
        Guid token);
    Task RegisterFromInvitationAsync(
        RegisterFromInvitationRequest request);
    Task<PagedResult<InvitationListItemResponse>>
GetInvitationsAsync(
    GetInvitationsRequest request);

}