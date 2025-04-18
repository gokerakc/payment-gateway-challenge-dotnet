using MediatR;
using PaymentGateway.Core;

namespace PaymentGateway.Application.Features.Payment.GetPayment;

public class GetPaymentHandler : IRequestHandler<GetPaymentQuery, GetPaymentResponse>
{
    private readonly IPaymentsRepository _paymentsRepository;

    public GetPaymentHandler(IPaymentsRepository paymentsRepository)
    {
        _paymentsRepository = paymentsRepository;
    }
    
    public Task<GetPaymentResponse> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var data = _paymentsRepository.Get(request.PaymentId);
        
        return data is null 
            ? Task.FromResult(GetPaymentResponse.NotFound() ) 
            : Task.FromResult(GetPaymentResponse.Success(data));
    }
}