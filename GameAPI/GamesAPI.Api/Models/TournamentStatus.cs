using System.ComponentModel.DataAnnotations;

namespace GamesAPI.Api.Models
{
    public class TournamentStatus
    {
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
            = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}