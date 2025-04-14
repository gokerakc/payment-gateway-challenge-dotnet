using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Features.Payment.Contracts;

using PaymentStatus = PaymentGateway.Api.Domain.Models.PaymentStatus;

namespace PaymentGateway.IntegrationTests.Features;

[Collection(ApplicationCollection.Name)]
public class PaymentEndpointTests
{
    private readonly Random _random = new();
    
    private readonly ApplicationFixture _fixture;
    private readonly JsonSerializerOptions _options;
    
    public PaymentEndpointTests(ApplicationFixture fixture)
    {
        _fixture = fixture;
        _options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
    }
    
    [Fact]
    public async Task GetPayment_WhenPaymentExist_ThenReturn200()
    {
        // Arrange
        var payment = CreateValidPayment();
        var paymentId = _fixture.PaymentsRepository.Add(payment);

        // Act
        var client = _fixture.CreateClient();
        var response = await client.GetAsync($"/api/payments/{paymentId}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>(_options);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        
        Assert.Equal(paymentResponse!.Id, paymentId);
        Assert.Equal(paymentResponse.Amount, payment.Amount);
        Assert.Equal(paymentResponse.ExpiryMonth, payment.ExpiryMonth);
        Assert.Equal(paymentResponse.ExpiryYear, payment.ExpiryYear);
        Assert.Equal(paymentResponse.Currency, payment.Currency);
        Assert.Equal(paymentResponse.CardNumberLastFour, payment.CardNumber[^4..]);
        Assert.Equal(paymentResponse.Status, Enum.Parse<Api.Features.Payment.Contracts.PaymentStatus>(payment.Status.ToString()));
    }

    [Fact]
    public async Task GetPayment_WhenPaymentNotFound_ThenReturn404()
    {
        // Arrange
        var client = _fixture.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task MakePayment_WhenBankAccepts_ThenReturn200WithAuthorizedStatus()
    {
        // Arrange
        var request = CreateValidPostPaymentRequest();
        var client = _fixture.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/payments", request);
        var result = await response.Content.ReadFromJsonAsync<PostPaymentResponse>(_options);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result!.Id);
        Assert.Equal(Api.Features.Payment.Contracts.PaymentStatus.Authorized, result.Status);
        Assert.Equal("1111", result.CardNumberLastFour);
        Assert.Equal(request.Amount, result.Amount);
        Assert.Equal(request.Currency, result.Currency);
        Assert.Equal(request.ExpiryMonth, result.ExpiryMonth);
        Assert.Equal(request.ExpiryYear, result.ExpiryYear);

        // Verify payment was stored
        var storedPayment = _fixture.PaymentsRepository.Get(result.Id);
        Assert.NotNull(storedPayment);
        Assert.Equal(result.Id, storedPayment!.Id);
        Assert.Equal(PaymentStatus.Authorized, storedPayment.Status);
    }
    
    [Fact]
    public async Task MakePayment_WhenBankDeclines_ThenReturn200WithDeclinedStatus()
    {
        // Arrange
        var request = CreateValidPostPaymentRequest();
        request.CardNumber = "4111111111111112"; // even numbers getting declined
        
        var client = _fixture.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/payments", request);
        var result = await response.Content.ReadFromJsonAsync<PostPaymentResponse>(_options);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result!.Id);
        Assert.Equal(Api.Features.Payment.Contracts.PaymentStatus.Declined, result.Status);
        Assert.Equal("1112", result.CardNumberLastFour);
        Assert.Equal(request.Amount, result.Amount);
        Assert.Equal(request.Currency, result.Currency);
        Assert.Equal(request.ExpiryMonth, result.ExpiryMonth);
        Assert.Equal(request.ExpiryYear, result.ExpiryYear);

        // Verify payment was stored
        var storedPayment = _fixture.PaymentsRepository.Get(result.Id);
        Assert.NotNull(storedPayment);
        Assert.Equal(result.Id, storedPayment!.Id);
        Assert.Equal(PaymentStatus.Declined, storedPayment.Status);
    }
    
    private Payment CreateValidPayment() => new Payment
    {
        CardNumber = "15298378940987367",
        ExpiryYear = _random.Next(2023, 2030),
        ExpiryMonth = _random.Next(1, 12),
        Amount = _random.Next(1, 10000),
        Currency = "GBP"
    };
    
    private PostPaymentRequest CreateValidPostPaymentRequest() => new PostPaymentRequest
    {
        CardNumber = "4111111111111111",
        ExpiryMonth = 12,
        ExpiryYear = DateTime.Now.Year + 1,
        Currency = "GBP",
        Amount = _random.Next(1, 10000),
        Cvv = "123"
    };
}