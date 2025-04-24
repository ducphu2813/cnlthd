using APIApplication.DTO.Invoice;

namespace APIApplication.Service.Interfaces;

public interface IInvoiceService
{
    Task<List<InvoiceDTO>> GetAll();
    Task<InvoiceDTO> GetById(Guid id);
    Task<InvoiceDTO> Add(SaveInvoiceDTO invoice);
    Task<InvoiceDTO> Update(Guid id, SaveInvoiceDTO invoice);
    Task<bool> Remove(Guid id);

    // lấy hóa đơn theo user id
    Task<List<InvoiceDTO>> GetByUserId(Guid userId);
}
