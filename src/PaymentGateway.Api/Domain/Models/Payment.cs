namespace PaymentGateway.Api.Domain.Models;

public class Payment
{
    public Guid Id { get; set; }
    
    public string CardNumber { get; set; } = String.Empty;
    
    public int Cvv { get; set; }
    
    public int ExpiryMonth { get; set; }
    
    public int ExpiryYear { get; set; }
    
    public string Currency { get; set; } = String.Empty;
    
    public int Amount { get; set; }
    
    public PaymentStatus Status { get; set; }
    
    // public string IdempotencyToken { get; set; } = String.Empty;
}