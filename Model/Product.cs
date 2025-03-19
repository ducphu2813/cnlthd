namespace APIApplication.Model;

public class Product
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public double? Price { get; set; }
    
    //quan hệ tới bảng InvoiceDetail
    public List<InvoiceDetail> InvoiceDetails { get; set; }
}