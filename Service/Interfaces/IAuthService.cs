using APIApplication.DTO.Auth;

namespace APIApplication.Service.Interfaces;

public interface IAuthService
{
    //đắng nhập
    Task<string> Login(LoginDTO loginDTO);
    
    //đăng ký 
    Task<string> Register(RegisterDTO registerDTO);
}