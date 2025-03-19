using APIApplication.DTO.InvoiceDetail;

namespace APIApplication.Service.Interfaces;

public interface IInvoiceDetailService
{
    Task<List<InvoiceDetailDTO>> GetAll();
    Task<InvoiceDetailDTO> GetById(Guid id);
    Task<InvoiceDetailDTO> Add(SaveInvoiceDetailDTO invoiceDetail);
    Task<InvoiceDetailDTO> Update(Guid id, SaveInvoiceDetailDTO invoiceDetail);
    Task<bool> Remove(Guid id);
    
    //lấy ra danh sách chi tiết hóa đơn theo id hóa đơn
    Task<List<InvoiceDetailDTO>> GetByInvoiceId(Guid invoiceId);
}