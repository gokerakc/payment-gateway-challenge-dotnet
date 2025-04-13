using PaymentGateway.Api.Common;

namespace PaymentGateway.Api.Domain.Models;

public record MakePaymentResult(Payment? Data, Status Status, string Message) : Result(Status, Message)
{
    public static MakePaymentResult Success(Payment data)
        => new MakePaymentResult(data, Status.Success, string.Empty);
    
    public static MakePaymentResult Error(string message)
        => new MakePaymentResult(Data: null, Status.Error, message);
}