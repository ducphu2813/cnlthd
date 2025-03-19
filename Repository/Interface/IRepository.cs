namespace APIApplication.Repository.Interface;

public interface IRepository <TEntity> : IDisposable where TEntity : class
{
    
    Task<IEnumerable<TEntity>> GetAll();
    Task<TEntity> GetById(Guid  id);
    Task<TEntity> Add(TEntity obj);
    Task<TEntity> Update(Guid  id, TEntity obj);
    Task<bool> Remove(Guid  id);
    Task<List<TEntity>> GetAllByIds(List<Guid > ids);
    
}