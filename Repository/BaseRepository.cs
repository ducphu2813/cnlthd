using System.Linq.Expressions;
using APIApplication.Context;
using APIApplication.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace APIApplication.Repository;

public class BaseRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    protected readonly DatabaseContext _context;

    public BaseRepository(DatabaseContext context)
    {
        _context = context;
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    //hàm lấy tất cả dữ liệu
    public async virtual Task<IEnumerable<TEntity>> GetAll()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public virtual async Task<TEntity> GetById(Guid id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public virtual async Task<TEntity> Add(TEntity obj)
    {
        //tự động tạo id cho object
        if (obj.GetType().GetProperty("Id") != null)
        {
            obj.GetType().GetProperty("Id").SetValue(obj, Guid.NewGuid());
        }

        await _context.Set<TEntity>().AddAsync(obj);
        await _context.SaveChangesAsync();
        return obj;
    }

    public virtual async Task<TEntity> Update(Guid id, TEntity obj)
    {
        _context.Set<TEntity>().Update(obj);
        await _context.SaveChangesAsync();
        return obj;
    }

    public virtual async Task<bool> Remove(Guid id)
    {
        var entity = await _context.Set<TEntity>().FindAsync(id);
        if (entity == null)
        {
            return false;
        }

        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public virtual async Task<List<TEntity>> GetAllByIds(List<Guid> ids)
    {
        return await _context
            .Set<TEntity>()
            .Where(x => ids.Contains((Guid)x.GetType().GetProperty("Id").GetValue(x, null)))
            .ToListAsync<TEntity>();
    }

    // hỗ trợ include các bảng liên quan
    public async Task<IEnumerable<TEntity>> GetAllWithIncludes(
        params Expression<Func<TEntity, object>>[] includes
    )
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return await query.ToListAsync();
    }
}
