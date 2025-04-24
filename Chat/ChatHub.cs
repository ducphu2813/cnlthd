using System.Security.Claims;
using APIApplication.Model;
using APIApplication.Service.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace APIApplication.Chat;

public class ChatHub : Hub
{
    private static readonly Dictionary<Guid, string> _userConnections = new(); // Lưu trữ UserId - ConnectionId
    private readonly IChatService _chatService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public ChatHub(IChatService chatService, IHttpContextAccessor httpContextAccessor)
    {
        _chatService = chatService;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (!_userConnections.ContainsKey(userId))
        {
            _userConnections.Add(userId, Context.ConnectionId);
        }
        else
        {
            _userConnections[userId] = Context.ConnectionId;
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(System.Exception exception)
    {
        var userId = GetUserId();
        if (_userConnections.ContainsKey(userId))
        {
            _userConnections.Remove(userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Gửi tin nhắn đến người khác
    public async Task SendMessage(Guid receiverId, string content)
    {
        var senderId = GetUserId();

        var message = new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        // Lưu vào DB
        await _chatService.SendMessageAsync(message);

        // Gửi tin nhắn đến người nhận nếu họ đang online
        if (_userConnections.TryGetValue(receiverId, out string receiverConnectionId))
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", new
            {
                senderId = senderId,
                content = content,
                sentAt = message.SentAt.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        // Optional: phản hồi lại người gửi
        await Clients.Caller.SendAsync("MessageSent", new
        {
            receiverId = receiverId,
            content = content,
            sentAt = message.SentAt.ToString("yyyy-MM-dd HH:mm:ss")
        });
    }

    // Hàm tiện ích: Lấy UserId từ JWT
    private Guid GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);
        return Guid.Parse(userIdClaim?.Value ?? throw new HubException("Unauthorized"));
    }
}