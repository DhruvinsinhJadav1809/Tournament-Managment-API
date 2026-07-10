using GamesAPI.Api.Enums;

namespace GamesAPI.Api.DTOs.Games
{

    public class GetInvitationsRequest
    {
        public string? Search { get; set; }

        // public InvitationStatus? Status { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}