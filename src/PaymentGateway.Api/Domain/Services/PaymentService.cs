using PaymentGateway.Api.Common;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Ports;
using PaymentGateway.Api.Ports.Clients;

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
        var processPaymentResult = await _bankApiClient.ProcessPayment(payment);
        switch (processPaymentResult.Status)
        {
            case Status.Success:
                payment.Status = PaymentStatus.Authorized;
                _paymentsRepository.Add(payment);
                return MakePaymentResult.Success(payment);
            case Status.Unauthorized:
                return MakePaymentResult.Unauthorized();
            case Status.Error:
                return MakePaymentResult.Error(processPaymentResult.Message);
            case Status.NotFound:
            default:
                throw new ArgumentOutOfRangeException();
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