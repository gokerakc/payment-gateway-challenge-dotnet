using PaymentGateway.Api.Common;

namespace PaymentGateway.Api.Domain.Models;

public record GetPaymentResult(Payment? Data, Status Status, string Message) : Result(Status, Message)
{
    public static GetPaymentResult Success(Payment data)
        => new GetPaymentResult(data, Status.Success, string.Empty);
    
    public static GetPaymentResult Error(string message)
        => new GetPaymentResult(Data: null, Status.Error, message);
}