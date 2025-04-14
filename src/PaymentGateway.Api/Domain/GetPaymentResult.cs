using PaymentGateway.Api.Common;
using PaymentGateway.Api.Domain.Models;

namespace PaymentGateway.Api.Domain;

public record GetPaymentResult(Payment? Data, Status Status, string Message) : Result(Status, Message)
{
    public static GetPaymentResult Success(Payment data)
        => new GetPaymentResult(data, Status.Success, string.Empty);
    
    public static GetPaymentResult NotFound()
        => new GetPaymentResult(null, Status.NotFound, "Payment not found");
}