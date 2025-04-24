using APIApplication.Model;

namespace APIApplication.Repository.Interface;

public interface IinvoiceRepository : IRepository<Invoice>
{
    // lấy hóa đơn theo user id
    Task<IEnumerable<Invoice>> GetByUserId(Guid userId);
}
