using PaymentGateway.Api.Common;
using PaymentGateway.Core;
using PaymentGateway.Core.Models;

namespace PaymentGateway.Api.Features.Payment.GetPayment;

public record GetPaymentResponse(Core.Models.Payment? Data, Status Status, string Message) : Result(Status, Message)
{
    public static GetPaymentResponse Success(Core.Models.Payment data)
        => new GetPaymentResponse(data, Status.Success, string.Empty);
    
    public static GetPaymentResponse NotFound()
        => new GetPaymentResponse(null, Status.NotFound, "Payment not found");
}