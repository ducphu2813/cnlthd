using APIApplication.Context;
using APIApplication.Model;
using APIApplication.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace APIApplication.Repository;

public class ChatMessageRepository : IChatMessageRepository
{
    private readonly DatabaseContext _context;

    public ChatMessageRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<ChatMessage>> GetMessagesBetweenUsersAsync(Guid currentId, Guid partnerId)
    {
        return await _context.ChatMessages
            .Where(m => (m.SenderId == currentId && m.ReceiverId == partnerId) ||
                        (m.SenderId == partnerId && m.ReceiverId == currentId))
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .ToListAsync();
    }

    public async Task<List<Guid>> GetChatPartnersAsync(Guid userId)
    {
        return await _context.ChatMessages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Distinct()
            .ToListAsync();
    }

    public async Task AddMessageAsync(ChatMessage message)
    {
        await _context.ChatMessages.AddAsync(message);
    }

    public async Task<List<Users>> GetAllAdminsAsync()
    {
        return await _context.Users
            .Where(u => u.Role == "ADMIN")
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}