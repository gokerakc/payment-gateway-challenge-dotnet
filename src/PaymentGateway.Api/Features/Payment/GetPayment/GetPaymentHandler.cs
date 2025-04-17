using MediatR;

using PaymentGateway.Core;

namespace PaymentGateway.Api.Features.Payment.GetPayment;

public class GetPaymentHandler : IRequestHandler<GetPaymentQuery, GetPaymentResponse>
{
    private readonly IPaymentsRepository _paymentsRepository;

    public GetPaymentHandler(IPaymentsRepository paymentsRepository)
    {
        _paymentsRepository = paymentsRepository;
    }
    
    public async Task<GetPaymentResponse> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var data = _paymentsRepository.Get(request.PaymentId);
        if (data is null)
        {
            return GetPaymentResponse.NotFound();
        }
        
        return GetPaymentResponse.Success(data);
    }
}