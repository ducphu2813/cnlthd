using APIApplication.DTO.Chat;
using APIApplication.DTO.Users;
using APIApplication.Model;
using APIApplication.Repository.Interface;
using APIApplication.Service.Interfaces;

namespace APIApplication.Service;

public class ChatService : IChatService
{
    private readonly IChatMessageRepository _chatMessageRepo;
    private readonly IUserRepository _userRepo;

    public ChatService(IChatMessageRepository chatMessageRepo, IUserRepository userRepo)
    {
        _chatMessageRepo = chatMessageRepo;
        _userRepo = userRepo;
    }

    public async Task<List<ChatMessageDTO>> GetMessagesBetweenAsync(Guid userId, Guid adminId)
    {
        List<ChatMessage> chatMessage = await _chatMessageRepo.GetMessagesBetweenUsersAsync(userId, adminId);
        
        if (chatMessage == null)
        {
            return new List<ChatMessageDTO>();
        }
        
        List<ChatMessageDTO> chatMessageDTOs = new List<ChatMessageDTO>();
        
        foreach (var message in chatMessage)
        {
            var sender = await _userRepo.GetById(message.SenderId);
            var receiver = await _userRepo.GetById(message.ReceiverId);

            chatMessageDTOs.Add(new ChatMessageDTO
            {
                UserName = message.SenderId.ToString() + " - " + message.ReceiverId.ToString(),
                Content = message.Content,
            });
        }
        
        return chatMessageDTOs;
    }

    public async Task SendMessageAsync(ChatMessage message)
    {
        await _chatMessageRepo.AddMessageAsync(message);
        await _chatMessageRepo.SaveChangesAsync();
    }

    public async Task<List<Guid>> GetChatPartnersAsync(Guid userId)
    {
        return await _chatMessageRepo.GetChatPartnersAsync(userId);
    }

    public async Task<List<Users>> GetAllAdminsAsync()
    {
        return await _chatMessageRepo.GetAllAdminsAsync();
    }
}