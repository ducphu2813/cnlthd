using APIApplication.DTO.InvoiceDetail;
using APIApplication.DTO.Users;

namespace APIApplication.DTO.Invoice;

public class InvoiceDTO
{
    public Guid Id { get; set; }
    public UserDTO Users { get; set; }
    public bool Status { get; set; }

    public List<InvoiceDetailDTO> InvoiceDetails { get; set; }
}
