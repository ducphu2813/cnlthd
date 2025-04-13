namespace APIApplication.DTO.InvoiceDetail;

public class UpdateQuantityDTO
{
    public Guid InvoiceId { get; set; }
    public Guid ProductId { get; set; }
    public int QuantityChange { get; set; } // dương -> tăng || âm -> giảm
}