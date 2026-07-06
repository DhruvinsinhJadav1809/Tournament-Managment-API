namespace GamesAPI.Api.Models
{
    public class Notification
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid? CreatedByUserId { get; set; }

        public User User { get; set; }

        public User? CreatedByUser { get; set; }
    }
}