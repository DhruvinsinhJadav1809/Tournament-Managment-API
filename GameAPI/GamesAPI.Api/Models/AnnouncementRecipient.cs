using System.ComponentModel.DataAnnotations;

namespace GamesAPI.Api.Models
{
    public class AnnouncementRecipient
    {
        public Guid Id { get; set; }

        public Guid AnnouncementId { get; set; }

        public Guid UserId { get; set; }

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Properties

        public Announcement Announcement { get; set; }

        public User User { get; set; }
    }
}