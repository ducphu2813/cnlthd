using APIApplication.DTO.Invoice;
using APIApplication.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIApplication.Controllers;


[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    
    private readonly IInvoiceService _invoiceService;
    
    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }
    
    //lấy tất cả các hóa đơn
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvoiceDTO>>> GetInvoices()
    {
        //lấy danh sách hóa đơn(DTO)
        var invoices = await _invoiceService.GetAll();
        
        return Ok(invoices);
    }
    
    //lấy hóa đơn theo id
    [HttpGet]
    [Route("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<ActionResult<InvoiceDTO>> GetInvoiceById(Guid id)
    {
        var invoice = await _invoiceService.GetById(id);
        
        if (invoice == null)
        {
            return NotFound();
        }
        
        return Ok(invoice);
    }
    
    //thêm hóa đơn
    [HttpPost]
    [Route("add")]
    public async Task<ActionResult<InvoiceDTO>> AddInvoice(SaveInvoiceDTO invoice)
    {
        return Ok(await _invoiceService.Add(invoice));
    }
    
    //cập nhật hóa đơn
    [HttpPut]
    [Route("update/{id}")]
    public async Task<ActionResult<InvoiceDTO>> UpdateInvoice(Guid id, SaveInvoiceDTO invoice)
    {
        return Ok(await _invoiceService.Update(id, invoice));
    }
    
    //xóa hóa đơn
    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<ActionResult<bool>> DeleteInvoice(Guid id)
    {
        return Ok(await _invoiceService.Remove(id));
    }
    
}