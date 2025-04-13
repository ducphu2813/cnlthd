using APIApplication.DTO.Payment;

namespace APIApplication.Service.Interfaces;

public interface IVnPayService
{
    Task<string> CreatePaymentUrl(Guid id, HttpContext context, DateTime expireDate);
    PaymentResponseDTO PaymentExecute(IQueryCollection collections);
    PaymentResponseDTO GetFullResponseData(IQueryCollection collection, string hashSecret);
}