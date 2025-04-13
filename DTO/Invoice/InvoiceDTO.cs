using APIApplication.DTO.InvoiceDetail;
using APIApplication.DTO.Users;

namespace APIApplication.DTO.Invoice;

public class InvoiceDTO
{
    public Guid Id { get; set; }
    public UserDTO Users { get; set; }
    
    //tổng tiền
    public double? TotalAmount { get; set; }
    
    public List<InvoiceDetailDTO> InvoiceDetails { get; set; }
}