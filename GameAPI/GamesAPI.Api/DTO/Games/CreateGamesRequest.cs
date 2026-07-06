using System.ComponentModel.DataAnnotations;

namespace GamesAPI.Api.DTOs.Games
{
    public class CreateGameRequest
    {
        [Required(ErrorMessage = "Game name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Participants per match is required.")]
        [Range(1, 100, ErrorMessage = "Participants per match must be greater than 0.")]
        public int? ParticipantsPerMatch { get; set; }
        public bool IsActive { get; set; } = true;
    }
}