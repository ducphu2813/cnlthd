using Microsoft.AspNetCore.SignalR;

namespace APIApplication.Chat;

public class NameIdentifierProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("sub")?.Value; // Hoặc claim nào bạn dùng làm ID
    }
}