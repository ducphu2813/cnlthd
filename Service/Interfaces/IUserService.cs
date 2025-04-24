using APIApplication.DTO.Users;
using APIApplication.Model;

namespace APIApplication.Service.Interfaces;

public interface IUserService
{
    Task<List<UserDTO>> GetAll();
    Task<UserDTO> GetById(Guid id);
    Task<UserDTO> Add(SaveUserDTO obj);
    Task<UserDTO> Update(Guid id, SaveUserDTO obj);
    Task<bool> Remove(Guid id);
    
    //tìm user theo role
    Task<List<UserDTO>> FindByRole(string role);
}