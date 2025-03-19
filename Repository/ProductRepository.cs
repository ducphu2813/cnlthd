using APIApplication.Context;
using APIApplication.Model;
using APIApplication.Repository.Interface;

namespace APIApplication.Repository;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    
    public ProductRepository(DatabaseContext context) : base(context)
    {
    }
    
}