using PaymentGateway.Api.Domain.Models;

namespace PaymentGateway.Api.Domain.Services;

public interface IPaymentService
{
    public Task<MakePaymentResult> MakePayment(Payment payment);
    
    public Task<GetPaymentResult> GetPayment(Guid paymentId);
}