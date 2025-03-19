using APIApplication.DTO.InvoiceDetail;
using APIApplication.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIApplication.Controllers;


[ApiController]
[Route("api/[controller]")]
public class InvoiceDetailController : ControllerBase
{
    
    private readonly IInvoiceDetailService _invoiceDetailService;
    
    public InvoiceDetailController(IInvoiceDetailService invoiceDetailService)
    {
        _invoiceDetailService = invoiceDetailService;
    }
    
    //lấy tất cả các chi tiết hóa đơn
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvoiceDetailDTO>>> GetInvoiceDetails()
    {
        var invoiceDetails = await _invoiceDetailService.GetAll();
        
        return Ok(invoiceDetails);
    }
    
    //lấy chi tiết hóa đơn theo invoice id
    [HttpGet]
    [Route("/invoice-id/{invoiceId}")]
    public async Task<ActionResult<IEnumerable<InvoiceDetailDTO>>> GetInvoiceDetailByInvoiceId(Guid invoiceId)
    {
        var invoiceDetails = await _invoiceDetailService.GetByInvoiceId(invoiceId);
        
        return Ok(invoiceDetails);
    }
    
    //lấy chi tiết hóa đơn theo id
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<InvoiceDetailDTO>> GetInvoiceDetailById(Guid id)
    {
        var invoiceDetail = await _invoiceDetailService.GetById(id);
        
        if (invoiceDetail == null)
        {
            return NotFound();
        }
        
        return Ok(invoiceDetail);
    }
    
    //thêm chi tiết hóa đơn
    [HttpPost]
    [Route("add")]
    public async Task<ActionResult<InvoiceDetailDTO>> AddInvoiceDetail(SaveInvoiceDetailDTO invoiceDetail)
    {
        //bắt exception nếu product hoặc invoice không tồn tại
        try
        {
            return Ok(await _invoiceDetailService.Add(invoiceDetail));
        }
        catch (System.Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
        
    }
    
    //cập nhật chi tiết hóa đơn
    [HttpPut]
    [Route("update/{id}")]
    public async Task<ActionResult<InvoiceDetailDTO>> UpdateInvoiceDetail(Guid id, SaveInvoiceDetailDTO invoiceDetail)
    {
        return Ok(await _invoiceDetailService.Update(id, invoiceDetail));
    }
    
    //xóa chi tiết hóa đơn
    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<ActionResult<bool>> DeleteInvoiceDetail(Guid id)
    {
        return Ok(await _invoiceDetailService.Remove(id));
    }
    
    //xóa chi tiết hóa đơn theo id hóa đơn
    [HttpDelete]
    [Route("delete-by-invoice-id/{invoiceId}")]
    public async Task<ActionResult<bool>> DeleteInvoiceDetailByInvoiceId(Guid invoiceId)
    {
        //tạm thời chưa xử lý
        return Ok(true);
    }
    
    
    
    
    
}