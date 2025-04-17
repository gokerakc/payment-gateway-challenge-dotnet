using PaymentGateway.Core.Models;

namespace PaymentGateway.Api.Clients.BankApiClient;

public interface IBankApiClient
{
    public Task<ProcessPaymentResult> ProcessPayment(Payment payment);
}