using APIApplication.Context;
using APIApplication.Model;
using APIApplication.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace APIApplication.Repository;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(DatabaseContext context) : base(context) { }

    // đây là hàm viết thêm ngoài BaseRepository, cũng cần phải giải thích rõ ràng tại sao lại viết như vậy
    public async Task<int?> UpdateQuantity(Guid id, int buyQuantity)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return -1;
        }
        else
        {
            product.Quantity -= buyQuantity;
            await _context.SaveChangesAsync();
            return product.Quantity;
        }
    }
    
    
    
    // ghi các hàm để làm quen với LINQ trong EntityFramework
    
    //tìm theo id
    public async Task<Product> GetById(Guid id)
    {
        var product = await _context //này là phần gọi _context
            .Products //này là phần gọi đến bảng Product
            .FindAsync(id); //đây là hàm LINQ để lấy bản ghi theo id
        
        
        if (product == null)
        {
            throw new System.Exception("Product not found");
        }
        return product;
    }
    
    //tìm theo tên(tìm đúng chính xác)
    public async Task<Product> GetByName(string name)
    {
        var product = await _context //này là phần gọi _context
            .Products //này là phần gọi đến bảng Product
            .FirstOrDefaultAsync(p => p.Name == name); //đây là hàm LINQ để lấy bản ghi theo tên
        
        if (product == null)
        {
            throw new System.Exception("Product not found");
        }
        return product;
    }
    
    
    //tìm theo tên (tìm gần đúng)
    public async Task<IEnumerable<Product>> GetByNameLike(string name)
    {
        var products = await _context //này là phần gọi _context
            .Products //này là phần gọi đến bảng Product
            .Where(p => p.Name.Contains(name)) //dùng Where để viết tìm kiếm theo điều kiện
                                                        // Contains là hàm LINQ để tìm kiếm gần đúng
            .ToListAsync(); //chuyển kết quả về dạng list
        
        if (products == null)
        {
            throw new System.Exception("Product not found");
        }
        return products;
    }
    
    //tìm theo giá trong khoảng
    public async Task<List<Product>> GetByPriceRange(double minPrice, double maxPrice)
    {
        var products = await _context //này là phần gọi _context
            .Products //này là phần gọi đến bảng Product
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice) //dùng Where để viết tìm kiếm theo điều kiện
            .ToListAsync(); //chuyển kết quả về dạng list
        
        if (products == null)
        {
            throw new System.Exception("Product not found");
        }
        return products;
    }
}
