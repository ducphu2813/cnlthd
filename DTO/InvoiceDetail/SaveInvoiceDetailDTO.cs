namespace APIApplication.DTO.InvoiceDetail;

public class SaveInvoiceDetailDTO
{
    public Guid InvoiceId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
