using GamesAPI.Api.Models;
using GamesAPI.Api.Models.Chat.Enums;
using System.ComponentModel.DataAnnotations;
public class Conversation
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public ConversationType Type { get; set; }

    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    // Navigation Properties

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual ICollection<ConversationParticipant> Participants { get; set; }
        = new List<ConversationParticipant>();

    public virtual ICollection<ConversationMessage> Messages { get; set; }
        = new List<ConversationMessage>();
}