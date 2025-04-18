using NSubstitute;
using PaymentGateway.Application.Common;
using PaymentGateway.Application.Features.Payment;
using PaymentGateway.Application.Features.Payment.CreatePayment;
using PaymentGateway.Core;
using PaymentGateway.Core.Models;

namespace PaymentGateway.UnitTests;

public class CreatePaymentHandlerTests
{
    private readonly IBankApiClient _mockBankApiClient = Substitute.For<IBankApiClient>();
    private readonly IPaymentsRepository _mockPaymentRepository = Substitute.For<IPaymentsRepository>();
    private readonly CreatePaymentHandler _sut;

    public CreatePaymentHandlerTests()
    {
        _sut = new CreatePaymentHandler(_mockBankApiClient, _mockPaymentRepository);
    }
    
    [Fact]
    public async Task Handle_WhenBankApproves_ReturnsSuccess()
    {
        // Arrange
        var payment = CreateValidPayment();
        var authorizationCode = Guid.NewGuid().ToString();
        
        _mockBankApiClient.ProcessPayment(Arg.Any<Payment>())
            .Returns(ProcessPaymentResult.Success(authorizationCode));
        
        _mockPaymentRepository.Add(Arg.Any<Payment>())
            .Returns(payment.Id);

        // Act
        var command = new CreatePaymentCommand(
            paymentId: payment.Id,
            amount: payment.Amount,
            expiryMonth: payment.ExpiryMonth,
            expiryYear: payment.ExpiryYear,
            currency: payment.Currency,
            cardNumber: payment.CardNumber,
            cvv: payment.Cvv
        );
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Status.Success, result.Status);
        Assert.Equal(payment.Id, result.Data?.Id);
        
        await _mockBankApiClient.Received(1).ProcessPayment(Arg.Any<Payment>());
        _mockPaymentRepository.Received(1).Add(Arg.Is<Payment>(p => 
            p.Id == payment.Id && 
            p.Status == PaymentStatus.Authorized));
    }

    [Fact]
    public async Task Handle_WhenBankDeclines_ReturnsUnauthorized()
    {
        // Arrange
        var payment = CreateValidPayment();
        
        _mockBankApiClient.ProcessPayment(Arg.Any<Payment>())
            .Returns(ProcessPaymentResult.Unauthorized());

        // Act
        var command = new CreatePaymentCommand(
            paymentId: payment.Id,
            amount: payment.Amount,
            expiryMonth: payment.ExpiryMonth,
            expiryYear: payment.ExpiryYear,
            currency: payment.Currency,
            cardNumber: payment.CardNumber,
            cvv: payment.Cvv
        );
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Status.Unauthorized, result.Status);
        
        await _mockBankApiClient.Received(1).ProcessPayment(Arg.Any<Payment>());
        _mockPaymentRepository.Received(1).Add(Arg.Is<Payment>(p => 
            p.Id == payment.Id && 
            p.Status == PaymentStatus.Declined));
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