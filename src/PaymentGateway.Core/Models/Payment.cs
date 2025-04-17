using PaymentGateway.Core.Validators;

namespace PaymentGateway.Core.Models;

public class Payment(
    Guid id,
    string cardNumber,
    string cvv,
    string currency,
    int expiryMonth,
    int expiryYear,
    decimal amount)
{
    public Guid Id { get; } = id;

    public string CardNumber { get; } = cardNumber;

    public string Cvv { get; } = cvv;

    public int ExpiryMonth { get; } = expiryMonth;

    public int ExpiryYear { get; } = expiryYear;

    public string Currency { get; } = currency;

    public decimal Amount { get; } = amount;

    public PaymentStatus Status { get; set; }


    public ValidationResult Validate() => PaymentValidator.Validate(this);
}