using PaymentGateway.Api.Endpoints.CreatePayment.Contracts;
using PaymentGateway.Core.Models;
using PaymentGateway.Core.Validators;

namespace PaymentGateway.UnitTests;

public class PaymentValidatorTests
{
    [Fact]
    public void Validate_WhenAllFieldsAreValid_ReturnsValidResult()
    {
        // Arrange
        // Arrange
        var payment = new Payment
        (
            id: Guid.NewGuid(),
            cardNumber: "4111111111111111",
            expiryMonth: 12,
            expiryYear: DateTime.Now.Year + 1,
            currency: "GBP",
            amount: 1000,
            cvv: "123"
        );

        // Act
        var result = PaymentValidator.Validate(payment);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("123")] 
    [InlineData("12345678901234567890")]
    [InlineData("41111111111111AA")]
    public void Validate_WhenCardNumberIsInvalid_ReturnsError(string cardNumber)
    {
        // Arrange
        var payment = new Payment
        (
            id: Guid.NewGuid(),
            cardNumber: cardNumber,
            expiryMonth: 12,
            expiryYear: DateTime.Now.Year + 1,
            currency: "GBP",
            amount: 1000,
            cvv: "123"
        );

        // Act
        var result = PaymentValidator.Validate(payment);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Key == nameof(PostPaymentRequest.CardNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    [InlineData(-1)]
    public void Validate_WhenExpiryMonthIsInvalid_ReturnsError(int month)
    {
        // Arrange
        var payment = new Payment
        (
            id: Guid.NewGuid(),
            cardNumber: "4111111111111111",
            expiryMonth: month,
            expiryYear: DateTime.Now.Year + 1,
            currency: "GBP",
            amount: 1000,
            cvv: "123"
        );

        // Act
        var result = PaymentValidator.Validate(payment);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Key == nameof(PostPaymentRequest.ExpiryMonth));
    }

    [Fact]
    public void Validate_WhenExpiryDateIsInPast_ReturnsError()
    {
        // Arrange
        var payment = new Payment
        (
            id: Guid.NewGuid(),
            cardNumber: "4111111111111111",
            expiryMonth: 12,
            expiryYear: DateTime.Now.Year - 1,
            currency: "GBP",
            amount: 1000,
            cvv: "123"
        );

        // Act
        var result = PaymentValidator.Validate(payment);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Key == nameof(PostPaymentRequest.ExpiryYear));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("USDD")]
    [InlineData("XX")]
    [InlineData("ABC")]
    public void Validate_WhenCurrencyIsInvalid_ReturnsError(string currency)
    {
        // Arrange
        var payment = new Payment
        (
            id: Guid.NewGuid(),
            cardNumber: "4111111111111111",
            expiryMonth: 12,
            expiryYear: DateTime.Now.Year + 1,
            currency: currency,
            amount: 1000,
            cvv: "1235678"
        );

        // Act
        var result = PaymentValidator.Validate(payment);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Key == nameof(PostPaymentRequest.Currency));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public void Validate_WhenAmountIsInvalid_ReturnsError(int amount)
    {
        // Arrange
        var payment = new Payment
        (
            id: Guid.NewGuid(),
            cardNumber: "4111111111111111",
            expiryMonth: 12,
            expiryYear: DateTime.Now.Year + 1,
            currency: "GBP",
            amount: amount,
            cvv: "1235678"
        );

        // Act
        var result = PaymentValidator.Validate(payment);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Key == nameof(PostPaymentRequest.Amount));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("12")]
    [InlineData("12345")]
    [InlineData("ABC")]
    public void Validate_WhenCvvIsInvalid_ReturnsError(string cvv)
    {
        // Arrange
        var payment = new Payment
        (
            id: Guid.NewGuid(),
            cardNumber: "4111111111111111",
            expiryMonth: 12,
            expiryYear: DateTime.Now.Year + 1,
            currency: "GBP",
            amount: 1000,
            cvv: cvv
        );

        // Act
        var result = PaymentValidator.Validate(payment);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Key == nameof(PostPaymentRequest.Cvv));
    }
}