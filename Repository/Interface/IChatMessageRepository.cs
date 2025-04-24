using APIApplication.Model;

namespace APIApplication.Repository.Interface;

public interface IChatMessageRepository
{
    Task<List<ChatMessage>> GetMessagesBetweenUsersAsync(Guid currentId, Guid partnerId);
    Task<List<Guid>> GetChatPartnersAsync(Guid userId);
    Task AddMessageAsync(ChatMessage message);
    Task<List<Users>> GetAllAdminsAsync();
    Task SaveChangesAsync();
}