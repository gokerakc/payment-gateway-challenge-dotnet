using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Ports;

namespace PaymentGateway.Api.Domain.Services;

public class PaymentValidator : IPaymentValidator
{
    public Task<bool> Validate(Payment payment)
    {
        return Task.FromResult(true);
    }
}