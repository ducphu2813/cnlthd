namespace APIApplication.Model;

public class ChatMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SenderId { get; set; }  // ID của người gửi
    public string ReceiverId { get; set; }  // ID của người nhận
    public string Message { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}