using MediatR;

namespace PaymentGateway.Application.Features.Payment.CreatePayment;

public class CreatePaymentCommand(
    Guid paymentId,
    string cardNumber,
    int expiryMonth,
    int expiryYear,
    string currency,
    decimal amount,
    string cvv)
    : IRequest<CreatePaymentResponse>
{
    public Guid PaymentId { get; } = paymentId;

    public string CardNumber { get; } = cardNumber;

    public int ExpiryMonth { get; } = expiryMonth;

    public int ExpiryYear { get; } = expiryYear;

    public string Currency { get; } = currency;

    public decimal Amount { get; } = amount;

    public string Cvv { get; } = cvv;
}