using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Features.Payment.Contracts;

public class PostPaymentResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("status")]
    public PaymentStatus Status { get; set; }
    
    [JsonPropertyName("cardNumberLastFour")]
    public string CardNumberLastFour { get; set; } = string.Empty;
    
    [JsonPropertyName("expiryMonth")]
    public int ExpiryMonth { get; set; }
    
    [JsonPropertyName("expiryYear")]
    public int ExpiryYear { get; set; }
    
    [JsonPropertyName("currency")]
    public string Currency { get; set; }
    
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
}
