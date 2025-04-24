using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using APIApplication.DTO.Chat;
using APIApplication.Model;
using APIApplication.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChatController(IChatService chatService, IHttpContextAccessor httpContextAccessor)
    {
        _chatService = chatService;
        _httpContextAccessor = httpContextAccessor;
    }

    // Lấy ID user hiện tại từ JWT
    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new System.Exception("Không tìm thấy claim 'sub' trong JWT.");
        }

        return Guid.Parse(userIdClaim.Value);
    }

    private string GetCurrentUserRole()
    {
        var roleClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role);
        return roleClaim?.Value ?? string.Empty;
    }

    /// <summary>
    /// Lấy tin nhắn giữa người dùng hiện tại và một user khác (admin hoặc user).
    /// </summary>
    [HttpGet("messages/{otherUserId}")]
    public async Task<IActionResult> GetMessagesWithUser(Guid otherUserId)
    {
        var currentUserId = GetCurrentUserId();
        var messages = await _chatService.GetMessagesBetweenAsync(currentUserId, otherUserId);
        return Ok(messages);
    }

    /// <summary>
    /// Gửi tin nhắn đến người dùng khác (admin hoặc user).
    /// </summary>
    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
    {
        var senderId = GetCurrentUserId();

        var message = new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            Content = dto.Content,
            SentAt = DateTime.UtcNow,
            IsRead = false
        };

        await _chatService.SendMessageAsync(message);
        return Ok(message);
    }

    /// <summary>
    /// Dành riêng cho Admin: Lấy danh sách user đã từng trò chuyện.
    /// </summary>
    [HttpGet("partners")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> GetChatPartners()
    {
        var currentAdminId = GetCurrentUserId();
        var partners = await _chatService.GetChatPartnersAsync(currentAdminId);
        return Ok(partners);
    }

    /// <summary>
    /// Lấy danh sách tất cả Admin (dành cho user dùng để gửi tin).
    /// </summary>
    [HttpGet("admins")]
    public async Task<IActionResult> GetAllAdmins()
    {
        var admins = await _chatService.GetAllAdminsAsync();
        return Ok(admins);
    }
}