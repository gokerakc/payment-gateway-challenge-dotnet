using System.Net.Http.Headers;

using PaymentGateway.Api.Clients.BankApiClient.Contract;
using PaymentGateway.Api.Constants;
using PaymentGateway.Core.Models;

namespace PaymentGateway.Api.Clients.BankApiClient;

public class BankApiClient : IBankApiClient
{
    private readonly HttpClient _client;
    private readonly ILogger<BankApiClient> _logger;
    
    public BankApiClient(
        IHttpClientFactory httpClientFactory,
        ILogger<BankApiClient> logger)
    {
        _client = httpClientFactory.CreateClient(ClientNames.BankApiClient);
        _logger = logger;
    }
    
    public async Task<ProcessPaymentResult> ProcessPayment(Payment payment)
    {
        var body = new BankApiProcessPaymentRequest
        {
            CardNumber = payment.CardNumber,
            ExpiryDate = $"{payment.ExpiryMonth}/{payment.ExpiryYear}",
            Currency = payment.Currency,
            Amount = payment.Amount,
            Cvv = payment.Cvv
        };
        
        try
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "/payments");
            message.Content = JsonContent.Create(body, mediaType: new MediaTypeHeaderValue("application/json"));
            var response = await _client.SendAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Bank API returned error status code {statusCode} for the payment. Payment Id: {paymentId}", 
                    response.StatusCode,
                    payment.Id);
                    
                return ProcessPaymentResult.Error("Acquiring bank could not process the payment at the moment");
            }
            
            var responseContent = await response.Content.ReadFromJsonAsync<BankApiProcessPaymentResponse>();

            return responseContent!.Authorized
                ? ProcessPaymentResult.Success(responseContent.AuthorizationCode)
                : ProcessPaymentResult.Unauthorized();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception occurred while processing payment. Payment Id: {paymentId}",
                payment.Id);
                
            return ProcessPaymentResult.Error("An unexpected error occurred while processing the payment");
        }
    }
}