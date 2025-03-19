using APIApplication.DTO;
using APIApplication.Model;

namespace APIApplication.Service.Interfaces;

public interface IProductService
{
    Task<List<ProductDTO>> GetAll();
    Task<ProductDTO> GetById(Guid id);
    Task<ProductDTO> Add(SaveProductDTO product);
    Task<ProductDTO> Update(Guid id, SaveProductDTO product);
    Task<bool> Remove(Guid id);
}