using APIApplication.DTO.Invoice;
using APIApplication.Model;

namespace APIApplication.DTO.InvoiceDetail;

public class InvoiceDetailDTO
{
    public Guid Id { get; set; }
    public ProductDTO Product { get; set; }
}