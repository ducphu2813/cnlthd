namespace APIApplication.DTO;

public class ProductDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double? Price { get; set; }
    
    //đường dẫn ảnh
    public string? ImageUrl { get; set; }
}