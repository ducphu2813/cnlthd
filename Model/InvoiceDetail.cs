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

    // số lượng sp
    public int Quantity { get; set; }

    // tổng tiền
    public double? Total { get; set; }
}
