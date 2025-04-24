using APIApplication.DTO.Auth;

namespace APIApplication.Service.Interfaces;

public interface IAuthService
{
    //đắng nhập
    Task<Dictionary<string, object>> Login(LoginDTO loginDTO);

    //đăng ký
    Task<string> Register(RegisterDTO registerDTO);
}
