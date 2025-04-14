namespace PaymentGateway.Api.Domain.Models;

public record ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public Dictionary<string, string[]> Errors { get; init; } = new();
}
