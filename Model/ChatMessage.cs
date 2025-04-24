namespace APIApplication.Model;

public class ChatMessage
{
    public Guid Id { get; set; }

    public Guid SenderId { get; set; }
    public Users Sender { get; set; }

    public Guid ReceiverId { get; set; }
    public Users Receiver { get; set; }

    public string Content { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; }
}