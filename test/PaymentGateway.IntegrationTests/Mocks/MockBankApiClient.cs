using PaymentGateway.Application.Features.Payment;
using PaymentGateway.Core.Models;

namespace PaymentGateway.IntegrationTests.Mocks;

public class MockBankApiClient : IBankApiClient
{
    public Task<ProcessPaymentResult> ProcessPayment(Payment payment)
    {
        var lastDigit = int.Parse(payment.CardNumber[^1].ToString());
        return Task.FromResult(lastDigit % 2 == 0 
            ? ProcessPaymentResult.Unauthorized() 
            : ProcessPaymentResult.Success(Guid.NewGuid().ToString()));
    }
}