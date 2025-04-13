namespace PaymentGateway.Api.Common;

public enum Status
{
    Success,
    NotFound,
    Unauthorized,
    Error
}

public abstract record Result(Status Status, string Message);