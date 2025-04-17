using PaymentGateway.Api.Common;
using PaymentGateway.Core;
using PaymentGateway.Core.Models;

namespace PaymentGateway.Api.Clients.BankApiClient;

public record ProcessPaymentResult(string AuthorizationCode, Status Status, string Message) : Result(Status, Message)
{
    public static ProcessPaymentResult Success(string authorizationCode)
    => new ProcessPaymentResult(authorizationCode, Status.Success, string.Empty);
    
    public static ProcessPaymentResult Unauthorized()
        => new ProcessPaymentResult(string.Empty, Status.Unauthorized, "The payment was declined by the call to the acquiring bank");
    
    public static ProcessPaymentResult Error(string message)
        => new ProcessPaymentResult(string.Empty, Status.Error, message);
}