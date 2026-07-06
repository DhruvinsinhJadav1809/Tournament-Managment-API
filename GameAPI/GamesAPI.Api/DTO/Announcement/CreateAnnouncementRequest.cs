using System.ComponentModel.DataAnnotations;

namespace GamesAPI.Api.DTOs
{
    public class CreateAnnouncementRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string Priority { get; set; } = string.Empty;

        [Required]
        public string TargetType { get; set; } = string.Empty;

        public Guid? TournamentId { get; set; }

        public Guid? MatchId { get; set; }

        public List<Guid>? UserIds { get; set; }

        public DateTime? ExpireAt { get; set; }
    }
}