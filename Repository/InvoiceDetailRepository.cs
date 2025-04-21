using APIApplication.Context;
using APIApplication.Model;
using APIApplication.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace APIApplication.Repository;

public class InvoiceDetailRepository : BaseRepository<InvoiceDetail>, IInvoiceDetailRepository
{
    public InvoiceDetailRepository(DatabaseContext context)
        : base(context) { }

    public override async Task<IEnumerable<InvoiceDetail>> GetAll()
    {
        return await _context
            .InvoiceDetails.Include(d => d.Product)
            .Include(d => d.Invoice)
            .ToListAsync();
    }

    //lấy tất cả invoice detail theo invoice id
    public async Task<IEnumerable<InvoiceDetail>> GetByInvoiceId(Guid invoiceId)
    {
        return await _context
            .InvoiceDetails.Where(d => d.InvoiceId == invoiceId)
            .Include(d => d.Product)
            .Include(d => d.Invoice)
            .ToListAsync();
    }

    // lấy invoice detail theo invoice id và product id
    public async Task<InvoiceDetail> GetByInvoiceIdAndProductId(Guid invoiceId, Guid productId)
    {
        return await _context
            .InvoiceDetails.Where(d => d.InvoiceId == invoiceId && d.ProductId == productId)
            .Include(d => d.Product)
            .Include(d => d.Invoice)
            .FirstOrDefaultAsync();
    }

    // lấy danh sách chi tiết hóa đơn theo id hóa đơn
    public async Task<IEnumerable<InvoiceDetail>> GetInvoiceDetailsByInvoiceId(Guid invoiceId)
    {
        return await _context
            .InvoiceDetails.Where(d => d.InvoiceId == invoiceId)
            .Include(d => d.Product)
            .Include(d => d.Invoice)
            .ToListAsync();
    }

    // xóa
    public async Task<bool> Remove(Guid id)
    {
        var invoiceDetail = await _context.InvoiceDetails.FindAsync(id);
        if (invoiceDetail == null)
        {
            return false;
        }

        _context.InvoiceDetails.Remove(invoiceDetail);
        await _context.SaveChangesAsync();
        return true;
    }
}
