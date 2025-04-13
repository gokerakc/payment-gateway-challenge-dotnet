using PaymentGateway.Api.Domain.Models;

namespace PaymentGateway.Api.Ports;

public interface IPaymentValidator
{
    public Task<bool> Validate(Payment payment);
}