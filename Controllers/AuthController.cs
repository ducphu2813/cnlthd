using APIApplication.DTO.Auth;
using APIApplication.Service.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    
    public AuthController(IAuthService authService
                            , IEmailService emailService
                            , IBackgroundJobClient backgroundJobClient)
    {
        _authService = authService;
        _emailService = emailService;
        _backgroundJobClient = backgroundJobClient;
    }
    
    //đăng nhập
    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<ActionResult<Dictionary<string, object>>> Login([FromBody] LoginDTO loginDTO)
    {
        try
        {
            return Ok(await _authService.Login(loginDTO));
        }
        catch (System.Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    //đăng ký
    [HttpPost]
    [Route("register")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> Register([FromBody] RegisterDTO registerDTO)
    {
        //test gửi mail ta sẽ đưa vào background job theo kiểu fire and forget
        _backgroundJobClient.Enqueue(
            () => _emailService.SendEmailAsync(registerDTO.Email, 
                "Đăng ký tài khoản thành công"
                , "Chúc mừng bạn đã đăng ký tài khoản thành công"));
        
        return Ok();
    }
}