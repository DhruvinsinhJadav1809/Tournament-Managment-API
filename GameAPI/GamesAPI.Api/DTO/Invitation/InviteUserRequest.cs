using System.ComponentModel.DataAnnotations;

namespace GamesAPI.Api.DTOs.Games
{

    public class InviteUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } = string.Empty;

    }
}