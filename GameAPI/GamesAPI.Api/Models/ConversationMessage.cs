using System.ComponentModel.DataAnnotations;
using GamesAPI.Api.Models;
public class ConversationMessage
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Guid SenderId { get; set; }

    public string Message { get; set; } = string.Empty;

    public DateTime SentAt { get; set; }

    public bool IsEdited { get; set; }

    public DateTime? EditedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    // Navigation Properties

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;

    public virtual ICollection<ConversationParticipant> SeenByParticipants { get; set; }
        = new List<ConversationParticipant>();
}