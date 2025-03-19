using APIApplication.Model;

namespace APIApplication.Repository.Interface;

public interface IInvoiceDetailRepository : IRepository<InvoiceDetail>
{
    
    //lấy tất cả invoice detail theo invoice id
    Task<IEnumerable<InvoiceDetail>> GetByInvoiceId(Guid invoiceId);
}