using APIApplication.DTO.Auth;
using APIApplication.DTO.Users;
using APIApplication.JWT;
using APIApplication.Repository.Interface;
using APIApplication.Service.Interfaces;

namespace APIApplication.Service;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly TokenProvider _tokenProvider;
    
    public AuthService(IUserRepository userRepository
                        , TokenProvider tokenProvider)
    {
        _userRepository = userRepository;
        _tokenProvider = tokenProvider;
    }
    
    public async Task<Dictionary<string, object>> Login(LoginDTO loginDTO)
    {
        
        //tìm theo email và password
        var user = await _userRepository.FindByEmailAndPassword(loginDTO.Email, loginDTO.Password);
        
        if(user == null)
        {
            throw new System.Exception("Email hoặc mật khẩu không đúng");
        }
        
        //tạo token
        var token = _tokenProvider.Create(user);

        UserDTO userDTO = new UserDTO()
        {
            Id = user.Id,
            Email = user.Email
        };
        
        return new Dictionary<string, object>()
        {
            {"token", token},
            {"user", userDTO}
        };
    }

    public Task<string> Register(RegisterDTO registerDTO)
    {
        throw new System.NotImplementedException();
    }
}