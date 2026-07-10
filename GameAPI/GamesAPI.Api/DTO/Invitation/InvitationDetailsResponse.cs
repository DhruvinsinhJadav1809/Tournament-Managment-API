namespace GamesAPI.Api.DTOs.Games
{

    public class InvitationDetailsResponse
    {

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }
    }
}