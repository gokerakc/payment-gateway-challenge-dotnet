namespace PaymentGateway.Api.Domain.Models;

public record ValidationResult
{
    public bool IsValid => !Errors.Any();
    public Dictionary<string, string[]> Errors { get; init; } = new();
}
