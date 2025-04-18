namespace PaymentGateway.Application.Common;

public abstract record Result(Status Status, string Message);