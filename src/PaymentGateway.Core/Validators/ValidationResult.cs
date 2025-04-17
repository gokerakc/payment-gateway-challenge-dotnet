namespace PaymentGateway.Core.Validators;

public record ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public Dictionary<string, string[]> Errors { get; init; } = new();
}
