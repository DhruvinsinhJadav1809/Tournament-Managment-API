using System.ComponentModel.DataAnnotations;
using GamesAPI.Api.Models;

public class ConversationParticipant
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Guid UserId { get; set; }

    public DateTime JoinedAt { get; set; }

    public bool IsAdmin { get; set; }

    public Guid? LastSeenMessageId { get; set; }

    // Navigation Properties

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ConversationMessage? LastSeenMessage { get; set; }
}