using System.ComponentModel.DataAnnotations;

namespace GamesAPI.Api.Models
{
    public class Announcement
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public string TargetType { get; set; } = string.Empty;

        public Guid? TournamentId { get; set; }

        public Guid? MatchId { get; set; }

        public DateTime? ExpireAt { get; set; }

        public Guid CreatedByUserId { get; set; }

        // Navigation Properties

        public Tournament? Tournament { get; set; }

        public TournamentMatch? Match { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User CreatedByUser { get; set; }

        public ICollection<AnnouncementRecipient>
            AnnouncementRecipients
        { get; set; }
                = new List<AnnouncementRecipient>();
    }
}