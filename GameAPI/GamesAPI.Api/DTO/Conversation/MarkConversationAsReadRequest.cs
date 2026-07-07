
public class MarkConversationAsReadRequest
{
    public Guid ConversationId { get; set; }

    public Guid LastSeenMessageId { get; set; }
}