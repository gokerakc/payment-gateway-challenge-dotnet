using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Features.Payment.Contracts;
using PaymentGateway.Api.Infrastructure.Storage;
using PaymentGateway.Api.Ports;

using PaymentStatus = PaymentGateway.Api.Features.Payment.Contracts.PaymentStatus;

namespace PaymentGateway.IntegrationTests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();
    
    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new Payment
        {
            CardNumber = "15298378940987367",
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            Currency = "GBP"
        };

        var paymentsRepository = new PaymentsRepository();
        var paymentId = paymentsRepository.Add(payment);

        var webApplicationFactory = new WebApplicationFactory<Program>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton<IPaymentsRepository>(_ => paymentsRepository)))
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

        // Act
        var response = await client.GetAsync($"/api/payments/{paymentId}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>(new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        });
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
        
        Assert.Equal(paymentResponse!.Id, paymentId);
        Assert.Equal(paymentResponse.Amount, payment.Amount);
        Assert.Equal(paymentResponse.ExpiryMonth, payment.ExpiryMonth);
        Assert.Equal(paymentResponse.ExpiryYear, payment.ExpiryYear);
        Assert.Equal(paymentResponse.Currency, payment.Currency);
        Assert.Equal(paymentResponse.CardNumberLastFour, payment.CardNumber[^4..]);
        Assert.Equal(paymentResponse.Status, Enum.Parse<PaymentStatus>(payment.Status.ToString()));
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<Program>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}