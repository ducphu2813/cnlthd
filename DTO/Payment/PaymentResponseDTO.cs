namespace APIApplication.DTO.Payment;

public class PaymentResponseDTO
{
    public string InvoiceDescription { get; set; }
    public string TransactionId { get; set; }
    public string InvoiceId { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentId { get; set; }
    public bool Success { get; set; }
    public string Token { get; set; }
    public string VnPayResponseCode { get; set; }
    public long? TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}