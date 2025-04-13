namespace APIApplication.Service.Interfaces;

public interface IJobTestService
{
    void SendMail();
    void CheckDatabase();
    void DeleteRecord();
    void UpdateRecord();
}