using APIApplication.DTO.Auth;
using APIApplication.DTO.Users;
using APIApplication.JWT;
using APIApplication.Repository.Interface;
using APIApplication.Service.Interfaces;
using AutoMapper;

namespace APIApplication.Service;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly TokenProvider _tokenProvider;
    private readonly IMapper _mapper;
    
    public AuthService(IUserRepository userRepository
                        , TokenProvider tokenProvider
                        , IMapper mapper)
    {
        _userRepository = userRepository;
        _tokenProvider = tokenProvider;
        _mapper = mapper;
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

        //dùng mapper
        //chuyển từ Model về DTO
        var userDTO = _mapper.Map<UserDTO>(user);
        
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