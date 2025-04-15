using PaymentGateway.Api.Domain.Models;

namespace PaymentGateway.Api.Ports.Services;

public interface IPaymentService
{
    public Task<MakePaymentResult> MakePayment(Payment payment);
    
    public Task<GetPaymentResult> GetPayment(Guid paymentId);
}