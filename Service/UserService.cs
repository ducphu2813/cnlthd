using APIApplication.DTO.Users;
using APIApplication.Exception;
using APIApplication.Model;
using APIApplication.Repository.Interface;
using APIApplication.Service.Interfaces;
using AutoMapper;

namespace APIApplication.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    
    public UserService(IUserRepository userRepository
                        , IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }


    public async Task<List<UserDTO>> GetAll()
    {
        var users = await _userRepository.GetAll();
        
        //chuyển từ List<Model> về List<DTO>
        return _mapper.Map<List<UserDTO>>(users);
    }

    public async Task<UserDTO> GetById(Guid id)
    {
        var user = await _userRepository.GetById(id);
        
        if (user == null)
        {
            throw new NotFoundException($"Không tìm thấy người dùng với ID = {id}");
        }
        
        //chuyển từ Model về DTO
        return _mapper.Map<UserDTO>(user);
    }

    public async Task<UserDTO> Add(SaveUserDTO obj)
    {
        //chuyển từ DTO về Model
        var userModel = _mapper.Map<Users>(obj);
        
        await _userRepository.Add(userModel);
        
        //chuyển từ Model về DTO
        return _mapper.Map<UserDTO>(userModel);
    }

    public async Task<UserDTO> Update(Guid id, SaveUserDTO obj)
    {
        //chuyển từ DTO về Model
        var userModel = _mapper.Map<Users>(obj);
        
        //set id cũ cho obj
        userModel.Id = id;
        
        await _userRepository.Update(id, userModel);
        
        //chuyển từ Model về DTO
        return _mapper.Map<UserDTO>(userModel);
    }

    public async Task<bool> Remove(Guid id)
    {
        return await _userRepository.Remove(id);
    }
    
    //tìm user theo role
    public async Task<List<UserDTO>> FindByRole(string role)
    {
        var users = await _userRepository.FindByRole(role);
        
        //chuyển từ List<Model> về List<DTO>
        return _mapper.Map<List<UserDTO>>(users);
    }
}