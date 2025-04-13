using PaymentGateway.Api.Common;

namespace PaymentGateway.Api.Ports.Clients;

public record ProcessPaymentResult(string AuthorizationCode, Status Status, string Message) : Result(Status, Message)
{
    public static ProcessPaymentResult Success(string authorizationCode)
    => new ProcessPaymentResult(authorizationCode, Status.Success, string.Empty);
    
    public static ProcessPaymentResult Error(string authorizationCode, string message)
        => new ProcessPaymentResult(authorizationCode, Status.Error, message);
}