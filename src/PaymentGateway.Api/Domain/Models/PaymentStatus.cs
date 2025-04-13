namespace PaymentGateway.Api.Domain.Models;

public enum PaymentStatus
{
    None,
    Authorized,
    Declined,
    Rejected
}