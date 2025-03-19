using APIApplication.Model;

namespace APIApplication.Repository.Interface;

public interface IUserRepository : IRepository<Users>
{
 
    //tìm user bằng email và password
    Task<Users> FindByEmailAndPassword(string email, string password);
}