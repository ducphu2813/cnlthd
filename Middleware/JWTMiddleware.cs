using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace APIApplication.Middleware;

public class JWTMiddleware
{
    private readonly RequestDelegate _next;
    
    public JWTMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context)
    {
        
        // Danh sách endpoint bỏ qua xác thựcnote
        var bypassEndpoints = new List<string> { "/api/auth/login", "/api/auth/register", "/api/product" };
        
        // Nếu request thuộc các endpoint này thì bỏ qua xác thực
        if (bypassEndpoints.Contains(context.Request.Path.Value?.ToLower()))
        {
            await _next(context);
            return;
        }

        var authenticationService = context.RequestServices.GetRequiredService<IAuthenticationService>();
        var result = await authenticationService.AuthenticateAsync(context, JwtBearerDefaults.AuthenticationScheme);

        if (!result.Succeeded)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                message = "Unauthorized"
            }));
            return;
        }

        await _next(context);
    }
}