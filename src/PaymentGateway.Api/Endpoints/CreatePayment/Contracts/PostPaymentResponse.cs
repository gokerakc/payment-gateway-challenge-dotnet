﻿using System.Text.Json.Serialization;

using PaymentGateway.Api.Endpoints.Shared.Contract;

namespace PaymentGateway.Api.Endpoints.CreatePayment.Contracts;

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
    public string Currency { get; set; } = string.Empty;
    
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}
