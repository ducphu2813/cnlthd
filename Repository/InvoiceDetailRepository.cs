using APIApplication.Context;
using APIApplication.Model;
using APIApplication.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace APIApplication.Repository;

public class InvoiceDetailRepository : BaseRepository<InvoiceDetail>, IInvoiceDetailRepository
{
    
    public InvoiceDetailRepository(DatabaseContext context) : base(context)
    {
    }
    
    public override async Task<IEnumerable<InvoiceDetail>> GetAll()
    {
        return await _context.InvoiceDetails
            .Include(d => d.Product)
            .Include(d => d.Invoice)
            .ToListAsync();
    }
    
    //lấy tất cả invoice detail theo invoice id
    public async Task<IEnumerable<InvoiceDetail>> GetByInvoiceId(Guid invoiceId)
    {
        return await _context.InvoiceDetails
            .Where(d => d.InvoiceId == invoiceId)
            .Include(d => d.Product)
            .Include(d => d.Invoice)
            .ToListAsync();
    }

    public async Task<bool> IsProductInInvoiceAsync(Guid invoiceId, Guid productId)
    {
        return await _context.InvoiceDetails
            .AnyAsync(d => d.InvoiceId == invoiceId && d.ProductId == productId);
    }

    public async Task<InvoiceDetail> GetByInvoiceAndProductIdAsync(Guid invoiceId, Guid productId)
    {
        return await _context.InvoiceDetails
            .FirstOrDefaultAsync(d => d.InvoiceId == invoiceId && d.ProductId == productId);
    }
}