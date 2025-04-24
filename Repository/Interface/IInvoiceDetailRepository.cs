using APIApplication.Model;

namespace APIApplication.Repository.Interface;

public interface IInvoiceDetailRepository : IRepository<InvoiceDetail>
{
    
    //lấy tất cả invoice detail theo invoice id
    Task<IEnumerable<InvoiceDetail>> GetByInvoiceId(Guid invoiceId);
    
    //lấy invoice detail theo product id và invoice id
    Task<InvoiceDetail> GetByProductIdAndInvoiceId(Guid productId, Guid invoiceId);

    // lấy invoice detail theo invoice id và product id
    Task<InvoiceDetail> GetByInvoiceIdAndProductId(Guid invoiceId, Guid productId);

    // lấy danh sách chi tiết hóa đơn theo id hóa đơn
    Task<IEnumerable<InvoiceDetail>> GetInvoiceDetailsByInvoiceId(Guid invoiceId);
}