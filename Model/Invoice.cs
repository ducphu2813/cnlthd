namespace APIApplication.Model;

public class Invoice
{
    public Guid Id { get; set; }
    
    //khóa ngoại đến bảng Users
    public Guid UserId { get; set; }
    public Users Users { get; set; }
    
    //quan hệ tới bảng InvoiceDetail
    public List<InvoiceDetail> InvoiceDetails { get; set; }
}