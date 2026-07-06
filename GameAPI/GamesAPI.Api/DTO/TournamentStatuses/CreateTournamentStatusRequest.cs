using System.ComponentModel.DataAnnotations;

namespace GamesAPI.Api.DTOs.TournamentStatuses
{
    public class CreateTournamentStatusRequest
    {
        [Required(ErrorMessage = "Status name is required.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
