namespace APIApplication.Model;

public class Invoice
{
    public Guid Id { get; set; }

    //khóa ngoại đến bảng Users
    public Guid UserId { get; set; }
    public Users Users { get; set; }
    public bool Status { get; set; } = false;

    // tổng tiền hóa đơn
    public double? TotalAmount { get; set; } = 0;

    //quan hệ tới bảng InvoiceDetail
    public List<InvoiceDetail> InvoiceDetails { get; set; }
}
