using PaymentGateway.Api.Common;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Ports.Clients.BankApiClient;
using PaymentGateway.Api.Ports.Storage;

namespace PaymentGateway.Api.Domain.Services;

public class PaymentService : IPaymentService
{
    private readonly IBankApiClient _bankApiClient;
    private readonly IPaymentsRepository _paymentsRepository;

    public PaymentService(IBankApiClient bankApiClient, IPaymentsRepository paymentsRepository)
    {
        _bankApiClient = bankApiClient;
        _paymentsRepository = paymentsRepository;
    }

    public async Task<MakePaymentResult> MakePayment(Payment payment)
    {
        var existingPayment = _paymentsRepository.Get(payment.Id);
        if (existingPayment != null)
        {
            return MakePaymentResult.Conflict(existingPayment);
        }
        
        var processPaymentResult = await _bankApiClient.ProcessPayment(payment);
        switch (processPaymentResult.Status)
        {
            case Status.Success:
                payment.Status = PaymentStatus.Authorized;
                _paymentsRepository.Add(payment);
                return MakePaymentResult.Success(payment);
            case Status.Unauthorized:
                payment.Status = PaymentStatus.Declined;
                _paymentsRepository.Add(payment);
                return MakePaymentResult.Unauthorized(payment);
            case Status.NotFound:
            case Status.Error:
            default:
                return MakePaymentResult.Error(processPaymentResult.Message);
        }
    }

    public async Task<GetPaymentResult> GetPayment(Guid paymentId)
    {
        var data = _paymentsRepository.Get(paymentId);
        if (data is null)
        {
            return GetPaymentResult.NotFound();
        }
        
        return GetPaymentResult.Success(data);
    }
}