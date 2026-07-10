using GamesAPI.Api.Enums;

namespace GamesAPI.Api.DTOs.Games
{
    public class InvitationListItemResponse
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public InvitationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime? AcceptedAt { get; set; }
    }
}