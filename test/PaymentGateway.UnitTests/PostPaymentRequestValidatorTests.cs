using PaymentGateway.Api.Features.Payment;
using PaymentGateway.Api.Features.Payment.Contracts;

namespace PaymentGateway.UnitTests;

public class PostPaymentRequestValidatorTests
{
    private readonly PostPaymentRequestValidator _sut = new();

    [Fact]
    public async Task Validate_WhenAllFieldsAreValid_ReturnsValidResult()
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "4111111111111111",
            ExpiryMonth = 12,
            ExpiryYear = DateTime.Now.Year + 1,
            Currency = "USD",
            Amount = 1000,
            Cvv = "123"
        };

        // Act
        var result = await _sut.Validate(request);

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
    public async Task Validate_WhenCardNumberIsInvalid_ReturnsError(string cardNumber)
    {
        // Arrange
        var request = CreateValidRequest();
        request.CardNumber = cardNumber;

        // Act
        var result = await _sut.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Key == nameof(PostPaymentRequest.CardNumber));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    [InlineData(-1)]
    public async Task Validate_WhenExpiryMonthIsInvalid_ReturnsError(int month)
    {
        // Arrange
        var request = CreateValidRequest();
        request.ExpiryMonth = month;

        // Act
        var result = await _sut.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Key == nameof(PostPaymentRequest.ExpiryMonth));
    }

    [Fact]
    public async Task Validate_WhenExpiryDateIsInPast_ReturnsError()
    {
        // Arrange
        var request = CreateValidRequest();
        request.ExpiryYear = DateTime.Now.Year - 1;
        request.ExpiryMonth = 1;

        // Act
        var result = await _sut.Validate(request);

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
    public async Task Validate_WhenCurrencyIsInvalid_ReturnsError(string currency)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Currency = currency;

        // Act
        var result = await _sut.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Key == nameof(PostPaymentRequest.Currency));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public async Task Validate_WhenAmountIsInvalid_ReturnsError(int amount)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Amount = amount;

        // Act
        var result = await _sut.Validate(request);

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
    public async Task Validate_WhenCvvIsInvalid_ReturnsError(string cvv)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Cvv = cvv;

        // Act
        var result = await _sut.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Key == nameof(PostPaymentRequest.Cvv));
    }

    private static PostPaymentRequest CreateValidRequest()
    {
        return new PostPaymentRequest
        {
            CardNumber = "4111111111111111",
            ExpiryMonth = 12,
            ExpiryYear = DateTime.Now.Year + 1,
            Currency = "GBP",
            Amount = 1000,
            Cvv = "123"
        };
    }
}