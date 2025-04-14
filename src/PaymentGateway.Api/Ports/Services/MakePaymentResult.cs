using PaymentGateway.Api.Common;
using PaymentGateway.Api.Domain.Models;

namespace PaymentGateway.Api.Ports.Services;

public record MakePaymentResult(Payment? Data, Status Status, string Message) : Result(Status, Message)
{
    public static MakePaymentResult Success(Payment data)
        => new MakePaymentResult(data, Status.Success, string.Empty);
    
    public static MakePaymentResult Unauthorized(Payment data)
        => new MakePaymentResult(data, Status.Unauthorized, "The payment was declined by the call to the acquiring bank");
    
    public static MakePaymentResult Error(string message)
        => new MakePaymentResult(Data: null, Status.Error, message);
}