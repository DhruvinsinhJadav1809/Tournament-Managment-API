using System.ComponentModel.DataAnnotations;
namespace GamesAPI.Api.DTOs.Games
{

    public class RegisterFromInvitationRequest
    {
        [Required]
        public Guid Token { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}