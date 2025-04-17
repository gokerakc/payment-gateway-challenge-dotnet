namespace PaymentGateway.Api.Common;

public enum Status
{
    Success,
    NotFound,
    Unauthorized,
    Conflict,
    Invalid,
    Error
}

public abstract record Result(Status Status, string Message);