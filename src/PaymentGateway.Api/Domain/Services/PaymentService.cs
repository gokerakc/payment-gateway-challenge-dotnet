using PaymentGateway.Api.Common;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Ports;
using PaymentGateway.Api.Ports.Clients;

namespace PaymentGateway.Api.Domain.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentValidator _paymentValidator;
    private readonly IBankApiClient _bankApiClient;
    private readonly IPaymentsRepository _paymentsRepository;

    public PaymentService(IPaymentValidator paymentValidator, IBankApiClient bankApiClient, IPaymentsRepository paymentsRepository)
    {
        _paymentValidator = paymentValidator;
        _bankApiClient = bankApiClient;
        _paymentsRepository = paymentsRepository;
    }

    public async Task<MakePaymentResult> MakePayment(Payment payment)
    {
        if (!await _paymentValidator.Validate(payment))
            return MakePaymentResult.Error("Payment validation failed");

        var processPaymentResult = await _bankApiClient.ProcessPayment(payment);
        if (processPaymentResult.Status is not Status.Success)
        {
            return MakePaymentResult.Error(processPaymentResult.Message);
        }   
        
        payment.Status = PaymentStatus.Authorized;
        
        _paymentsRepository.Add(payment);

        return MakePaymentResult.Success(payment);
    }

    public async Task<GetPaymentResult> GetPayment(Guid paymentId)
    {
        var data = _paymentsRepository.Get(paymentId);
        if (data is null)
        {
            return GetPaymentResult.Error("Payment not found");
        }
        
        return GetPaymentResult.Success(data);
    }
}