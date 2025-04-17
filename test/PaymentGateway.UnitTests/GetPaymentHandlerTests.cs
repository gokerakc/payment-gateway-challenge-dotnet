using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using PaymentGateway.Api.Clients.BankApiClient;
using PaymentGateway.Api.Common;
using PaymentGateway.Api.Features.Payment.GetPayment;
using PaymentGateway.Core;
using PaymentGateway.Core.Models;

namespace PaymentGateway.UnitTests;

public class GetPaymentHandlerTests
{
    private readonly IPaymentsRepository _mockPaymentRepository = Substitute.For<IPaymentsRepository>();
    private readonly GetPaymentHandler _sut;

    public GetPaymentHandlerTests()
    {
        _sut = new GetPaymentHandler(_mockPaymentRepository);
    }
    
    [Fact]
    public async Task Handle_WhenPaymentExists_ReturnsPayment()
    {
        // Arrange
        var payment = CreateValidPayment();
        _mockPaymentRepository.Get(payment.Id).Returns(payment);

        // Act
        var result = await _sut.Handle(new GetPaymentQuery(payment.Id), CancellationToken.None);

        // Assert
        Assert.Equal(Status.Success, result.Status);
        Assert.Equal(payment.Id, result.Data?.Id);
        Assert.Equal(payment.Amount, result.Data?.Amount);
        Assert.Equal(payment.CardNumber, result.Data?.CardNumber);
    }

    [Fact]
    public async Task Handle_WhenPaymentNotFound_ReturnsNotFound()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        _mockPaymentRepository.Get(paymentId).Returns((Payment?)null);

        // Act
        var result = await _sut.Handle(new GetPaymentQuery(paymentId), CancellationToken.None);

        // Assert
        Assert.Equal(Status.NotFound, result.Status);
        Assert.Null(result.Data);
    }

    private static Payment CreateValidPayment()
    {
        return new Payment
        (
            id: Guid.NewGuid(),
            cardNumber: "4111111111111111",
            expiryMonth: 12,
            expiryYear: DateTime.Now.Year + 1,
            currency: "GBP",
            amount: 1000,
            cvv: "123"
        );
    }
}