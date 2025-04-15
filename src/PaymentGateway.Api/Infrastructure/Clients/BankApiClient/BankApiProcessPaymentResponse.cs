using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Infrastructure.Clients.BankApiClient;

public class BankApiProcessPaymentResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }
    
    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; } = string.Empty;
}
