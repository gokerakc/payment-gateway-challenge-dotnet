using MediatR;

namespace PaymentGateway.Application.Features.Payment.GetPayment;

public class GetPaymentQuery(Guid paymentId) : IRequest<GetPaymentResponse>
{
    public Guid PaymentId { get; } = paymentId;
}