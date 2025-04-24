using APIApplication.Model;

namespace APIApplication.Repository.Interface;

public interface IUserRepository : IRepository<Users>
{
 
    //tìm user bằng email và password, đây là hàm viết thêm ngoài BaseRepository, cần giải thích cả hàm này rõ ràng dễ hiểu
    Task<Users> FindByEmailAndPassword(string email, string password);
    
    //tìm user theo role
    Task<List<Users>> FindByRole(string role);
}