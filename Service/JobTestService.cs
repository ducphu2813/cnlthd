using APIApplication.Repository.Interface;
using APIApplication.Service.Interfaces;

namespace APIApplication.Service;

public class JobTestService : IJobTestService
{
    
    private readonly ILogger _logger;
    
    public JobTestService(ILogger<JobTestService> logger)
    {
        _logger = logger;
    }
    
    public void SendMail()
    {
        _logger.LogInformation("Vừa send mail thành công");
    }

    public void CheckDatabase()
    {
        _logger.LogInformation("Check Database, database đang hoạt động bình thường");
    }

    public void DeleteRecord()
    {
        _logger.LogInformation("Đã xóa record thành công");
    }

    public void UpdateRecord()
    {
        _logger.LogInformation("Đã update record theo mong muốn");
    }
}