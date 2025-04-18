using PaymentGateway.Application.Common;

namespace PaymentGateway.Application.Features.Payment.CreatePayment;

public record CreatePaymentResponse(Core.Models.Payment? Data, Status Status, string Message, Dictionary<string, string[]>? Errors = null) : Result(Status, Message)
{
    public static CreatePaymentResponse Success(Core.Models.Payment data)
        => new CreatePaymentResponse(data, Status.Success, string.Empty);
    
    public static CreatePaymentResponse Unauthorized(Core.Models.Payment data)
        => new CreatePaymentResponse(data, Status.Unauthorized, "The payment was declined by the call to the acquiring bank");
    
    public static CreatePaymentResponse Conflict(Core.Models.Payment data)
        => new CreatePaymentResponse(data, Status.Conflict, string.Empty);
    
    public static CreatePaymentResponse ValidationError(string message, Dictionary<string, string[]> errors)
        => new CreatePaymentResponse(Data: null, Status.Invalid, message, errors);
    
    public static CreatePaymentResponse Error(string message)
        => new CreatePaymentResponse(Data: null, Status.Error, message);
}