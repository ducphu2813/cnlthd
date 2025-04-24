namespace APIApplication.DTO.Chat;

public class SendMessageDto
{
    public Guid ReceiverId { get; set; }
    public string Content { get; set; }
}