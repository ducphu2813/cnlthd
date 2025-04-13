using APIApplication.DTO.Payment;
using APIApplication.Library;
using APIApplication.Repository.Interface;
using APIApplication.Service.Interfaces;

namespace APIApplication.Service;

public class VnPayService : IVnPayService
{
    
    private readonly IConfiguration _configuration;
    private readonly IinvoiceRepository _invoiceRepository;
    
    //logger
    private readonly ILogger<VnPayService> _logger;
    
    public VnPayService(IConfiguration configuration
                        , IinvoiceRepository invoiceRepository
                        , ILogger<VnPayService> logger)
    {
        _configuration = configuration;
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }
    
    public async Task<string> CreatePaymentUrl(Guid id, HttpContext context, DateTime expireDate)
    {
        _logger.LogInformation("invoice id là: {InvoiceId}", id);
        
        // Guid invoiceId = Guid.Empty;
        // if (model.InvoiceID != Guid.Empty)
        // {
        //     invoiceId = model.InvoiceID;
        // }
        // else
        // {
        //     throw new System.Exception("Invoice ID is empty");
        // }
        
        //kiểm tra invoice id có tồn tại hay không
        var invoice = await _invoiceRepository.GetById(id);
        
        _logger.LogInformation("đã lấy data xong");
        
        if(invoice == null)
            throw new System.Exception("Invoice not found");
        
        _logger.LogInformation("invoice status is: {Status}", invoice.Status);
        
        //kiểm tra status của invoice
        if (string.IsNullOrEmpty(invoice.Status) || !invoice.Status.Trim().ToUpper().Equals("PENDING"))
        {
            _logger.LogWarning("Invoice status is null or not PENDING. Actual: '{Status}'", invoice.Status);
            throw new System.Exception("Invoice can not be paid");
        }
        
        var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
        
        //kiểm tra expireDate có là utc hay không
        if(expireDate.Kind != DateTimeKind.Utc)
        {
            expireDate = expireDate.ToUniversalTime();
        }
        
        var expiryTime = TimeZoneInfo.ConvertTimeFromUtc(expireDate, timeZoneById);
        
        var urlCallBack = "http://localhost:5173/payment/payment_result";
        
        var pay = new VnPayLibrary();
        pay.AddRequestData("vnp_Version", _configuration["VnPay:Version"]);
        pay.AddRequestData("vnp_Command", _configuration["VnPay:Command"]);
        pay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
        pay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"]);
        pay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]);
        pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
        pay.AddRequestData("vnp_TxnRef", timeNow.Ticks.ToString());
        pay.AddRequestData("vnp_OrderInfo", id.ToString());
        pay.AddRequestData("vnp_OrderType", id.ToString());
        pay.AddRequestData("vnp_Amount", (invoice.TotalAmount * 100).ToString());
        pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
        pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
        
        // Truyền thời gian hết hạn vào tham số vnp_ExpireDate
        pay.AddRequestData("vnp_ExpireDate", expiryTime.ToString("yyyyMMddHHmmss"));
        
        var paymentUrl = 
            pay.CreateRequestUrl(_configuration["VnPay:Url"], _configuration["VnPay:HashSecret"]);
        
        return paymentUrl;
    }

    public PaymentResponseDTO PaymentExecute(IQueryCollection collections)
    {
        var pay = new VnPayLibrary();
        var response = 
            pay.GetFullResponseData(collections, _configuration["VnPay:HashSecret"]);

        return response;
    }

    public PaymentResponseDTO GetFullResponseData(IQueryCollection collection, string hashSecret)
    {
        var vnPay = new VnPayLibrary();

        foreach (var (key, value) in collection)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                vnPay.AddResponseData(key, value);
            }
        }
        
        var orderId = Convert.ToString(vnPay.GetResponseData("vnp_TxnRef"));
        var vnPayTranId = Convert.ToString(vnPay.GetResponseData("vnp_TransactionNo"));
        var vnpResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
        var vnpSecureHash =
            collection.FirstOrDefault(k => k.Key == "vnp_SecureHash").Value; //hash của dữ liệu trả về
        var orderInfo = vnPay.GetResponseData("vnp_OrderInfo");
        DateTime date;
        if (DateTime.TryParseExact(vnPay.GetResponseData("vnp_PayDate"), "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out date))
        {
            Console.WriteLine("Date: " + date);
        }
        else
        {
            Console.WriteLine("Invalid date format.");
        }
        var checkSignature =
            vnPay.ValidateSignature(vnpSecureHash, hashSecret); //check Signature

        if (!checkSignature)
            return new PaymentResponseDTO()
            {
                Success = false
            };
        
        //kiểm tra trạng thái thanh toán
        if (vnpResponseCode != "00")
        {
            _logger.LogError("VnPay payment failed with response code: {ResponseCode}", vnpResponseCode);
            
            return new PaymentResponseDTO()
            {
                Success = false,
                VnPayResponseCode = vnpResponseCode,
                InvoiceDescription = orderInfo,
            };
            
        }
        
        //nếu thành công thì cập nhật trạng thái thanh toán
        _logger.LogInformation("id lấy được từ vnp_TxnRef: {orderId}", orderId);
        _logger.LogInformation("id lấy được từ vnp_OrderInfo: {orderInfo}", orderInfo);

        return new PaymentResponseDTO()
        {
            Success = true,
            PaymentMethod = "VnPay",
            InvoiceDescription = orderInfo,
            InvoiceId = orderInfo,
            PaymentId = vnPayTranId.ToString(),
            TransactionId = vnPayTranId.ToString(),
            Token = vnpSecureHash,
            VnPayResponseCode = vnpResponseCode,
            TotalAmount = Convert.ToInt64(vnPay.GetResponseData("vnp_Amount")) / 100,
            CreatedAt = date
        };
    }
}