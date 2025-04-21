using APIApplication.Context;
using APIApplication.Model;
using APIApplication.Repository.Interface;

namespace APIApplication.Repository;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(DatabaseContext context)
        : base(context) { }

    // public async Task<int?> UpdateQuantity(Guid id, int buyQuantity)
    // {
    //     var product = await _context.Products.FindAsync(id);
    //     if (product == null)
    //     {
    //         return -1;
    //     }
    //     else
    //     {
    //         product.Quantity -= buyQuantity;
    //         await _context.SaveChangesAsync();
    //         return product.Quantity;
    //     }
    // }
}
