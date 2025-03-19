namespace APIApplication.Exception;

public class ProductNotFoundException : NotFoundException
{
    
    public ProductNotFoundException(Guid productId) 
        : base($"Không tìm thấy sản phẩm với ID: {productId}") { }
    
}