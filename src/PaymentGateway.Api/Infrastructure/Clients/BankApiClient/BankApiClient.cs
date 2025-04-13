using System.Net.Http.Headers;

using PaymentGateway.Api.Constants;
using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Ports.Clients;

namespace PaymentGateway.Api.Infrastructure.Clients.BankApiClient;

public class BankApiClient : IBankApiClient
{
    private readonly HttpClient _client;
    
    public BankApiClient(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient(ClientNames.BankApiClient);
    }
    
    public async Task<ProcessPaymentResult> ProcessPayment(Payment payment)
    {
        var body = new BankApiProcessPaymentRequest
        {
            CardNumber = payment.CardNumber,
            ExpiryDate = $"{payment.ExpiryMonth}/{payment.ExpiryYear}",
            Currency = payment.Currency,
            Amount = payment.Amount,
            Cvv = payment.Cvv.ToString()
        };
        
        var message = new HttpRequestMessage(HttpMethod.Post, "/payments");
        message.Content = JsonContent.Create(body, mediaType: new MediaTypeHeaderValue("application/json"));
        var response = await _client.SendAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            return ProcessPaymentResult.Error(string.Empty, "An error occured while processing payment.");
        }
        
        var responseContent = await response.Content.ReadFromJsonAsync<BankApiProcessPaymentResponse>();
        return ProcessPaymentResult.Success(responseContent!.AuthorizationCode);
    }
}