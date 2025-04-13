using APIApplication.Context;
using APIApplication.DTO;
using APIApplication.Model;
using APIApplication.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIApplication.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    
    private readonly IProductService _productService;
    private readonly IPhotoService _photoService;
    
    //inject logging
    private readonly ILogger<ProductController> _logger;
    
    public ProductController(IProductService productService
                            , IPhotoService photoService
                            , ILogger<ProductController> logger)
    {
        _productService = productService;
        _photoService = photoService;
        _logger = logger;
    }
    
    //lấy tất cả các sản phẩm
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts()
    {
        //lấy danh sách sản phẩm(DTO)
        var products = await _productService.GetAll();
        
        return Ok(products);
    }
    
    //lấy sản phẩm theo id
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<ProductDTO>> GetProductById(Guid id)
    {
        var product = await _productService.GetById(id);
        
        if (product == null)
        {
            return NotFound();
        }
        
        return Ok(product);
    }
    
    //thêm sản phẩm
    [HttpPost]
    [Route("add")]
    public async Task<ActionResult<ProductDTO>> AddProduct(SaveProductDTO product)
    {
        string imageUrl = null;
        
        //nếu có ảnh thì upload ảnh lên cloudinary
        if (product.Image != null)
        {
            var uploadResult = await _photoService.UploadPhotoAsync(product.Image);
            if (uploadResult.Error != null) return BadRequest(uploadResult.Error.Message);
            
            imageUrl = uploadResult.SecureUrl.AbsoluteUri;
        }
        
        return Ok(await _productService.Add(product, imageUrl));
    }
    
    //cập nhật sản phẩm
    [HttpPut]
    [Route("update/{id}")]
    public async Task<ActionResult<ProductDTO>> UpdateProduct(Guid id, SaveProductDTO product)
    {
        string imageUrl = null;
        
        //nếu có ảnh thì upload ảnh lên cloudinary
        if (product.Image != null)
        {
            //upload ảnh mới lên cloudinary và lấy url
            var uploadResult = await _photoService.UploadPhotoAsync(product.Image);
            
            //kiểm tra kết quả upload ảnh có lỗi không
            if (uploadResult.Error != null) return BadRequest(uploadResult.Error.Message);
            
            //lấy url ảnh mới, lấy absoluteUri
            imageUrl = uploadResult.SecureUrl.AbsoluteUri;
            _logger.LogInformation("Upload ảnh mới thành công");
            
            //tìm sản phẩm cũ và xóa ảnh cũ
            var oldProduct = await _productService.GetById(id);
            
            //kiểm tra nếu sản phẩm cũ có ảnh thì mới xóa ảnh
            if (oldProduct != null && oldProduct.ImageUrl != null)
            {
                _logger.LogInformation("Sản phẩm có ảnh cũ");
                
                var publicId = _photoService.ExtractPublicId(oldProduct.ImageUrl);
                await _photoService.DeletePhotoAsync(publicId);
                
                _logger.LogInformation("Xóa ảnh cũ thành công");
            }
        }
        
        _logger.LogInformation("Cập nhật sản phẩm");
        return Ok(await _productService.Update(id, product, imageUrl));
    }
    
    //xóa sản phẩm
    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<ActionResult<bool>> DeleteProduct(Guid id)
    {
        //tìm sản phẩm và xóa ảnh
        var product = await _productService.GetById(id);
        
        if (product != null || product.ImageUrl != null)
        {
            var publicId = _photoService.ExtractPublicId(product.ImageUrl);
            await _photoService.DeletePhotoAsync(publicId);
        }
        
        return Ok(await _productService.Remove(id));
    }
}