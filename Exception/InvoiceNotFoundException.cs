namespace APIApplication.Exception;

public class InvoiceNotFoundException : NotFoundException
{
    
    public InvoiceNotFoundException(Guid invoiceId) 
        : base($"Không tìm thấy hóa đơn với ID: {invoiceId}") { }
    
}