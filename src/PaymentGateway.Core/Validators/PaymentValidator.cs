using System.Globalization;
using System.Text.RegularExpressions;

using PaymentGateway.Core.Models;

namespace PaymentGateway.Core.Validators;

public static class PaymentValidator
{
    private static readonly string[] ValidCurrencies = ["USD", "EUR", "GBP"];
    
    public static ValidationResult Validate(Payment request)
    {
        var result = new ValidationResult();

        ValidateCardNumber(request.CardNumber, result);
        ValidateExpiryDate(request.ExpiryMonth, request.ExpiryYear, result);
        ValidateCurrency(request.Currency, result);
        ValidateAmount(request.Amount, result);
        ValidateCvv(request.Cvv, result);

        return result;
    }

    private static void ValidateCardNumber(string cardNumber, ValidationResult result)
    {
        const string propertyName = nameof(Payment.CardNumber);
        
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
            result.Errors.Add(nameof(Payment.ExpiryMonth), ["Expiry month must be between 1 and 12"]);
            return;
        }

        var today = DateTime.Today;
        var expiryDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        
        if (expiryDate <= today)
        {
            result.Errors.Add(nameof(Payment.ExpiryYear), ["Expiry year can not be lower than current year"]);
        }
    }

    private static void ValidateCurrency(string currency, ValidationResult result)
    {
        const string propertyName = nameof(Payment.Currency);

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

    private static void ValidateAmount(decimal amount, ValidationResult result)
    {
        if (amount <= 0)
        {
            result.Errors.Add(nameof(Payment.Amount), ["Amount must be greater than 0"]);
            return;
        }

        string amountStr = amount.ToString(CultureInfo.InvariantCulture);
        if (amountStr.Contains('.') || amountStr.Contains(','))
        {
            result.Errors.Add(nameof(Payment.Amount), ["Amount must be provided in minor currency units as a whole number. For example, £10.50 should be provided as 1050."]);
        }
    }

    private static void ValidateCvv(string cvv, ValidationResult result)
    {
        const string propertyName = nameof(Payment.Cvv);

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