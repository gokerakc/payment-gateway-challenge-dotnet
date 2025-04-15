using PaymentGateway.Api.Domain.Models;

namespace PaymentGateway.Api.Ports;

public interface IValidator<in T>
{
    Task<ValidationResult> Validate(T payment);
}