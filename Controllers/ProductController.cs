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

    public ProductController(IProductService productService)
    {
        _productService = productService;
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
        return Ok(await _productService.Add(product));
    }

    //cập nhật sản phẩm
    [HttpPut]
    [Route("update/{id}")]
    public async Task<ActionResult<ProductDTO>> UpdateProduct(Guid id, SaveProductDTO product)
    {
        return Ok(await _productService.Update(id, product));
    }

    //xóa sản phẩm
    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<ActionResult<bool>> DeleteProduct(Guid id)
    {
        return Ok(await _productService.Remove(id));
    }
}
