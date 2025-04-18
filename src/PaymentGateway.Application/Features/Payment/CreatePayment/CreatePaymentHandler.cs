using MediatR;
using PaymentGateway.Application.Common;
using PaymentGateway.Core;
using PaymentGateway.Core.Models;
using Serilog;

namespace PaymentGateway.Application.Features.Payment.CreatePayment;

public class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, CreatePaymentResponse>
{
    private readonly IBankApiClient _bankApiClient;
    private readonly IPaymentsRepository _paymentsRepository;
    
    private readonly ILogger _logger = Log.ForContext<CreatePaymentHandler>();

    public CreatePaymentHandler(IBankApiClient bankApiClient, IPaymentsRepository paymentsRepository)
    {
        _bankApiClient = bankApiClient;
        _paymentsRepository = paymentsRepository;
    }
    
    public async Task<CreatePaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var existingPayment = _paymentsRepository.Get(request.PaymentId);
        if (existingPayment != null)
        {
            return CreatePaymentResponse.Conflict(existingPayment);
        }
        
        var payment = MapToPayment(request);

        var validationResult = payment.Validate();
        if (!validationResult.IsValid)
        {
            return CreatePaymentResponse.ValidationError("Invalid payment details", validationResult.Errors);
        }
        
        var processPaymentResult = await _bankApiClient.ProcessPayment(payment);
        switch (processPaymentResult.Status)
        {
            case Status.Success:
                _logger.Information("Payment {paymentId} handled successfully", request.PaymentId);
                payment.Status = PaymentStatus.Authorized;
                _paymentsRepository.Add(payment);
                return CreatePaymentResponse.Success(payment);
            case Status.Unauthorized:
                _logger.Information("Payment {paymentId} handled successfully", request.PaymentId);
                payment.Status = PaymentStatus.Declined;
                _paymentsRepository.Add(payment);
                return CreatePaymentResponse.Unauthorized(payment);
            default:
                _logger.Error("An error occured while handling the payment: {paymentId}", request.PaymentId);
                return CreatePaymentResponse.Error(processPaymentResult.Message);
        }
    }

    private static Core.Models.Payment MapToPayment(CreatePaymentCommand request) => 
    new (request.PaymentId, request.CardNumber, request.Cvv, request.Currency, request.ExpiryMonth, request.ExpiryYear, request.Amount);
}