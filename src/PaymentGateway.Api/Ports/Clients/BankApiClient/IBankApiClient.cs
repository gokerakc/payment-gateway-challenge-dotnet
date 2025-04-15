using PaymentGateway.Api.Domain.Models;

namespace PaymentGateway.Api.Ports.Clients.BankApiClient;

public interface IBankApiClient
{
    public Task<ProcessPaymentResult> ProcessPayment(Payment payment);
}