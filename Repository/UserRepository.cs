using APIApplication.Context;
using APIApplication.Model;
using APIApplication.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace APIApplication.Repository;

public class UserRepository : BaseRepository<Users>, IUserRepository
{
    
    public UserRepository(DatabaseContext context) : base(context)
    {
    }
    
    //tìm user bằng email và password
    public async Task<Users> FindByEmailAndPassword(string email, string password)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
    }
    
    //tìm user theo role
    public async Task<List<Users>> FindByRole(string role)
    {
        return await _context.Users.Where(x => x.Role == role).ToListAsync();
    }
    
}