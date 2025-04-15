using System.Text.RegularExpressions;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Features.Payment.Contracts;
using PaymentGateway.Api.Ports;

namespace PaymentGateway.Api.Features.Payment;

public class PostPaymentRequestValidator : IValidator<PostPaymentRequest>
{
    private static readonly string[] ValidCurrencies = ["USD", "EUR", "GBP"];
    
    public Task<ValidationResult> Validate(PostPaymentRequest request)
    {
        var result = new ValidationResult();

        ValidateCardNumber(request.CardNumber, result);
        ValidateExpiryDate(request.ExpiryMonth, request.ExpiryYear, result);
        ValidateCurrency(request.Currency, result);
        ValidateAmount(request.Amount, result);
        ValidateCvv(request.Cvv, result);

        return Task.FromResult(result);
    }

    private static void ValidateCardNumber(string cardNumber, ValidationResult result)
    {
        const string propertyName = nameof(PostPaymentRequest.CardNumber);
        
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            result.Errors.Add(propertyName, ["Card number is required"]);
            return;
        }

        if (!Regex.IsMatch(cardNumber, @"^\d{14,19}$"))
        {
            result.Errors.Add(propertyName, ["Card number must be between 14 and 19 digits"]);
        }
    }

    private static void ValidateExpiryDate(int month, int year, ValidationResult result)
    {
        if (month is < 1 or > 12)
        {
            result.Errors.Add(nameof(PostPaymentRequest.ExpiryMonth), ["Expiry month must be between 1 and 12"]);
            return;
        }

        var today = DateTime.Today;
        var expiryDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        
        if (expiryDate <= today)
        {
            result.Errors.Add(nameof(PostPaymentRequest.ExpiryYear), ["Expiry year can not be lower than current year"]);
        }
    }

    private static void ValidateCurrency(string currency, ValidationResult result)
    {
        const string propertyName = nameof(PostPaymentRequest.Currency);

        if (string.IsNullOrWhiteSpace(currency))
        {
            result.Errors.Add(propertyName, ["Currency is required"]);
            return;
        }

        if (currency.Length != 3)
        {
            result.Errors.Add(propertyName, ["Currency must be 3 characters long"]);
            return;
        }

        if (!ValidCurrencies.Contains(currency.ToUpper()))
        {
            result.Errors.Add(propertyName, [$"Currency must be one of: {string.Join(", ", ValidCurrencies)}"]);
        }
    }

    private static void ValidateAmount(int amount, ValidationResult result)
    {
        if (amount <= 0)
        {
            result.Errors.Add(nameof(PostPaymentRequest.Amount), ["Amount must be greater than 0"]);
        }
    }

    private static void ValidateCvv(string cvv, ValidationResult result)
    {
        const string propertyName = nameof(PostPaymentRequest.Cvv);

        if (string.IsNullOrWhiteSpace(cvv))
        {
            result.Errors.Add(propertyName, ["CVV is required"]);
            return;
        }

        if (!Regex.IsMatch(cvv, @"^\d{3,4}$"))
        {
            result.Errors.Add(propertyName, ["CVV must be 3 or 4 digits"]);
        }
    }
}