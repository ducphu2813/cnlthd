using APIApplication.DTO.Chat;
using APIApplication.Model;

namespace APIApplication.Service.Interfaces;

public interface IChatService
{
    Task<List<ChatMessageDTO>> GetMessagesBetweenAsync(Guid userId, Guid adminId);
    Task SendMessageAsync(ChatMessage message);
    Task<List<Guid>> GetChatPartnersAsync(Guid userId);
    Task<List<Users>> GetAllAdminsAsync();
}