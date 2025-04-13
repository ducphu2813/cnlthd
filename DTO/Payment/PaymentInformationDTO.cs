namespace APIApplication.DTO.Payment;

public class PaymentInformationDTO
{
    public Guid OrderType { get; set; }
    public Guid InvoiceID { get; set; }
    public double? TotalAmount { get; set; }
    public string? InvoiceDescription { get; set; }
    public string? Name { get; set; }
    public string? UserId { get; set; }
}