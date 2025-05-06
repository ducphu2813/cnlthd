using APIApplication.DTO.Payment;
using APIApplication.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    
    private readonly IVnPayService _vnPayService;
    
    public PaymentController(IVnPayService vnPayService)
    {
        _vnPayService = vnPayService;
    }
    
    //gửi request để nhận link thanh toán
    [HttpPost]
    [Route("create-payment-url/{invoice_id}")]
    public async Task<ActionResult<string>> CreatePaymentUrl(Guid invoice_id)
    {
        var paymentUrl = await _vnPayService.CreatePaymentUrl(invoice_id, HttpContext, DateTime.Now.AddMinutes(15));
        
        return Ok(paymentUrl);
    }
    
    //xử lý sau khi thanh toán xong
    [HttpPost]
    [Route("handle-payment-result")]
    public async Task<ActionResult<string>> PaymentResult()
    {
        
        var result = await _vnPayService.PaymentExecute(HttpContext.Request.Query);
        
        return Ok(result);
    }
    
    
}