namespace PaymentGateway.Api.Common;

public enum Status
{
    Success,
    NotFound,
    BadRequest,
    Error
}

public abstract record Result(Status Status, string Message);