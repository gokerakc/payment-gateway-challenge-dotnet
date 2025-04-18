namespace PaymentGateway.Application.Features.Payment;

public interface IBankApiClient
{
    public Task<ProcessPaymentResult> ProcessPayment(Core.Models.Payment payment);
}