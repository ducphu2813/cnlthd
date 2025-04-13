namespace APIApplication.Model;

public class Invoice
{
    public Guid Id { get; set; }
    
    //khóa ngoại đến bảng Users
    public Guid UserId { get; set; } //phần này bắt buộc phải có
    public Users Users { get; set; } // phần này không bắt buộc
    
    //tổng tiền
    public double? TotalAmount { get; set; } = 0;
    
    //trạng thái thanh toán. Có 3 trạng thái: CANCELLED, PENDING, PAID
    public string Status { get; set; } = "PENDING";
    
    //thời gian tạo
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    //quan hệ tới bảng InvoiceDetail
    public List<InvoiceDetail> InvoiceDetails { get; set; }
}