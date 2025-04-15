using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Features.Payment.Contracts;

public class PostPaymentRequest
{
    [JsonPropertyName("cardNumber")]
    public required string CardNumber { get; set; }
    
    [JsonPropertyName("expiryMonth")]
    public required int ExpiryMonth { get; set; }
    
    [JsonPropertyName("expiryYear")]
    public required int ExpiryYear { get; set; }
    
    [JsonPropertyName("currency")]
    public required string Currency { get; set; }
    
    [JsonPropertyName("amount")]
    public required int Amount { get; set; }
    
    [JsonPropertyName("cvv")]
    public required string Cvv { get; set; }
}