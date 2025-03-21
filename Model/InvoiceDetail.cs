namespace APIApplication.Model;



public class InvoiceDetail
{
    public Guid Id { get; set; }
    
    //khóa ngoại đến bảng Invoice
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; }
    
    //khóa ngoại đến bảng Product
    public Guid ProductId { get; set; }
    public Product Product { get; set; }

    public int Quantity { get; set; }
    public double Total { get; set; }
}