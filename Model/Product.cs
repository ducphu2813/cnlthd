namespace APIApplication.Model;

public class Product
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double? Price { get; set; }

    public int? Quantity { get; set; }
    
    //đường dẫn ảnh
    public string? ImageUrl { get; set; }
    
    //quan hệ tới bảng InvoiceDetail
    public List<InvoiceDetail> InvoiceDetails { get; set; }
}