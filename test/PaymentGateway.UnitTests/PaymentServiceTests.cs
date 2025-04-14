using NSubstitute;
using PaymentGateway.Api.Common;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Domain.Services;
using PaymentGateway.Api.Ports;
using PaymentGateway.Api.Ports.Clients;

namespace PaymentGateway.UnitTests;

public class PaymentServiceTests
{
    private readonly IBankApiClient _mockBankApiClient = Substitute.For<IBankApiClient>();
    private readonly IPaymentsRepository _mockPaymentRepository = Substitute.For<IPaymentsRepository>();
    private readonly PaymentService _sut;

    public PaymentServiceTests()
    {
        _sut = new PaymentService(_mockBankApiClient, _mockPaymentRepository);
    }
    
    [Fact]
    public async Task MakePayment_WhenBankApproves_ReturnsSuccess()
    {
        // Arrange
        var payment = CreateValidPayment();
        var authorizationCode = Guid.NewGuid().ToString();
        
        _mockBankApiClient.ProcessPayment(payment)
            .Returns(ProcessPaymentResult.Success(authorizationCode));
        
        _mockPaymentRepository.Add(Arg.Any<Payment>())
            .Returns(payment.Id);

        // Act
        var result = await _sut.MakePayment(payment);

        // Assert
        Assert.Equal(Status.Success, result.Status);
        Assert.Equal(payment.Id, result.Data?.Id);
        
        await _mockBankApiClient.Received(1).ProcessPayment(payment);
        _mockPaymentRepository.Received(1).Add(Arg.Is<Payment>(p => 
            p.Id == payment.Id && 
            p.Status == PaymentStatus.Authorized));
    }

    [Fact]
    public async Task MakePayment_WhenBankDeclines_ReturnsUnauthorized()
    {
        // Arrange
        var payment = CreateValidPayment();
        
        _mockBankApiClient.ProcessPayment(payment)
            .Returns(ProcessPaymentResult.Unauthorized());

        // Act
        var result = await _sut.MakePayment(payment);

        // Assert
        Assert.Equal(Status.Unauthorized, result.Status);
        
        await _mockBankApiClient.Received(1).ProcessPayment(payment);
        _mockPaymentRepository.Received(1).Add(Arg.Is<Payment>(p => 
            p.Id == payment.Id && 
            p.Status == PaymentStatus.Declined));
    }

    [Fact]
    public async Task GetPayment_WhenPaymentExists_ReturnsPayment()
    {
        // Arrange
        var payment = CreateValidPayment();
        _mockPaymentRepository.Get(payment.Id).Returns(payment);

        // Act
        var result = await _sut.GetPayment(payment.Id);

        // Assert
        Assert.Equal(Status.Success, result.Status);
        Assert.Equal(payment.Id, result.Data?.Id);
        Assert.Equal(payment.Amount, result.Data?.Amount);
        Assert.Equal(payment.CardNumber, result.Data?.CardNumber);
    }

    [Fact]
    public async Task GetPayment_WhenPaymentNotFound_ReturnsNotFound()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        _mockPaymentRepository.Get(paymentId).Returns((Payment?)null);

        // Act
        var result = await _sut.GetPayment(paymentId);

        // Assert
        Assert.Equal(Status.NotFound, result.Status);
        Assert.Null(result.Data);
    }

    private static Payment CreateValidPayment()
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            CardNumber = "4111111111111111",
            ExpiryMonth = 12,
            ExpiryYear = DateTime.Now.Year + 1,
            Currency = "GBP",
            Amount = 1000,
            Cvv = "123"
        };
    }
}