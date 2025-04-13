using APIApplication.DTO.Users;
using APIApplication.Service.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace APIApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    
    private readonly IUserService _userService;
    
    //test backgroud job
    private readonly IJobTestService _jobTestService;
    
    //đây là 2 interface của hangfire
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager;
    
    public UserController(IUserService userService
                            , IJobTestService jobTestService
                            , IBackgroundJobClient backgroundJobClient
                            , IRecurringJobManager recurringJobManager)
    {
        _userService = userService;
        _jobTestService = jobTestService;
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
    }
    
    //lấy tất cả các user
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
    {
        //background job bằng schedule
        //job này sẽ chạy sau 1 khoảng thời gian tùy chỉnh
        var job1 = _backgroundJobClient.Schedule(
            () => _jobTestService.DeleteRecord(), 
            TimeSpan.FromSeconds(10)
            );
        
        //Đây là continution job
        //job này sẽ chạy sau khi 1 job được chỉ định trước nó chạy xong
        //ví dụ như job này sẽ chạy sau khi job1 khai báo ở trên chạy xong
        _backgroundJobClient.ContinueJobWith(
            job1, 
            () => _jobTestService.UpdateRecord()
            );
        
        //đây là backgroundjob theo kiểu fire and forget
        //nó sẽ chạy ngay khi hàm này được gọi
        _backgroundJobClient.Enqueue(() => _jobTestService.SendMail());
        
        //recurring job
        //job này sẽ chạy theo chu kỳ lặp lại mỗi 5 giây
        //job này 1 khi được khởi tạo thì sẽ chạy mãi mãi mà không cần gọi lại hàm này, vì nó đã được lưu trong database của hangfire
        _recurringJobManager.AddOrUpdate(
            "recurring-job", 
            () => _jobTestService.CheckDatabase(), 
            Cron.Minutely
            );
        
        //tất cả các background job trên để có thể bắt đầu chạy thì cần chạy thì hàm này cần được gọi
        
        return Ok(await _userService.GetAll());
    }
    
    //lấy user theo id
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<UserDTO>> GetUser([FromRoute] Guid id)
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
    public async Task<ActionResult<UserDTO>> AddUser([FromBody] SaveUserDTO user)
    {
        return Ok(await _userService.Add(user));
    }
    
    //cập nhật user
    [HttpPut]
    [Route("update/{id}")]
    public async Task<ActionResult<UserDTO>> UpdateUser(Guid id, [FromBody] SaveUserDTO user)
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