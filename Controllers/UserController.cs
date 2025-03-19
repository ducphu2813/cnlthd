using APIApplication.DTO.Users;
using APIApplication.Model;
using APIApplication.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    //lấy tất cả các user
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
    {
        return Ok(await _userService.GetAll());
    }
    
    //lấy user theo id
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<UserDTO>> GetUser(Guid id)
    {
        var user = await _userService.GetById(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
    
    //thêm user
    [HttpPost]
    [Route("add")]
    public async Task<ActionResult<UserDTO>> AddUser(SaveUserDTO user)
    {
        return Ok(await _userService.Add(user));
    }
    
    //cập nhật user
    [HttpPut]
    [Route("update/{id}")]
    public async Task<ActionResult<UserDTO>> UpdateUser(Guid id, SaveUserDTO user)
    {
        return Ok(await _userService.Update(id, user));
    }
    
    //xóa user
    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<ActionResult<bool>> DeleteUser(Guid id)
    {
        return Ok(await _userService.Remove(id));
    }
    
}