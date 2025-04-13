using APIApplication.DTO.Auth;
using APIApplication.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    
    public AuthController(IAuthService authService
                            , IEmailService emailService)
    {
        _authService = authService;
        _emailService = emailService;
    }
    
    //đăng nhập
    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<ActionResult<string>> Login([FromBody] LoginDTO loginDTO)
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
        //test gửi mail
        await _emailService.SendEmailAsync("gameservice4me@gmail.com", "Test", "Test gửi từ API");
        return Ok();
    }
}