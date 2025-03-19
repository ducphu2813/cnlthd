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
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
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
}