using MediatR;

namespace PaymentGateway.Api.Features.Payment.GetPayment;

public class GetPaymentQuery(Guid paymentId) : IRequest<GetPaymentResponse>
{
    public Guid PaymentId { get; } = paymentId;
}