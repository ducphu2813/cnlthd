using APIApplication.Context;
using APIApplication.Model;
using APIApplication.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace APIApplication.Repository;

public class InvoiceRepository : BaseRepository<Invoice>, IinvoiceRepository
{
    
    public InvoiceRepository(DatabaseContext context) : base(context)
    {
    }
    
    public override async Task<Invoice> GetById(Guid id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Users)
            .Include(i => i.InvoiceDetails)
            .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null)
        {
            throw new System.Exception("Invoice not found");
        }

        return invoice;
    }
    
    public override async Task<IEnumerable<Invoice>> GetAll()
    {
        return await _context.Invoices
            .Include(i => i.Users)
            .Include(i => i.InvoiceDetails)
            .ThenInclude(d => d.Product) // Include cả Product trong InvoiceDetails
            .ToListAsync();
    }
    
}