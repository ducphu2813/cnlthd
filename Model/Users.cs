namespace APIApplication.Model;

public class Users
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    
    //quan hệ tới bảng Invoice
    public List<Invoice> Invoices { get; set; }
}