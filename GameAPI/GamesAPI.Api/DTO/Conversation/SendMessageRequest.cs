public class SendMessageRequest
{
    public Guid ConversationId { get; set; }

    public string Message { get; set; } = string.Empty;
}