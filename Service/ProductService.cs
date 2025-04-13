using APIApplication.DTO;
using APIApplication.Model;
using APIApplication.Repository.Interface;
using APIApplication.Service.Interfaces;
using AutoMapper;

namespace APIApplication.Service;

public class ProductService : IProductService
{
    
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    
    public ProductService(IProductRepository productRepository
                            , IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }
    
    public async Task<List<ProductDTO>> GetAll()
    {
        var products = await _productRepository.GetAll();
        
        //chuyển từ List<Model> về List<DTO>
        return _mapper.Map<List<ProductDTO>>(products);
    }
    
    public async Task<ProductDTO> GetById(Guid id)
    {
        var product = await _productRepository.GetById(id);
        
        //chuyển từ Model về DTO
        return _mapper.Map<ProductDTO>(product);
    }
    
    public async Task<ProductDTO> Add(SaveProductDTO product, string imageUrl)
    {
        //chuyển từ DTO về Model
        var productModel = _mapper.Map<Product>(product);
        
        //gán đường dẫn ảnh
        productModel.ImageUrl = imageUrl;
        
        await _productRepository.Add(productModel);
        
        //chuyển từ Model về DTO
        return _mapper.Map<ProductDTO>(productModel);
    }
    
    public async Task<ProductDTO> Update(Guid id, SaveProductDTO product, string imageUrl)
    {
        //chuyển từ DTO về Model
        // var productModel = _mapper.Map<Product>(product);
        
        //tìm sản phẩm theo id
        var productModel = await _productRepository.GetById(id);
        
        //set id cũ cho obj
        productModel.Id = id;
        
        //set các giá trị mới
        productModel.Name = product.Name;
        productModel.Description = product.Description;
        productModel.Price = product.Price;
        
        //set đường dẫn ảnh
        productModel.ImageUrl = imageUrl;
        
        await _productRepository.Update(id, productModel);
        
        //chuyển từ Model về DTO
        return _mapper.Map<ProductDTO>(productModel);
    }
    
    public async Task<bool> Remove(Guid id)
    {
        return await _productRepository.Remove(id);
    }
    
}