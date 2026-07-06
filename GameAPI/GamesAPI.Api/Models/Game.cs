using System.ComponentModel.DataAnnotations;

namespace GamesAPI.Api.Models
{
    public class Game
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int ParticipantsPerMatch { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid? UpdatedByUserId { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
