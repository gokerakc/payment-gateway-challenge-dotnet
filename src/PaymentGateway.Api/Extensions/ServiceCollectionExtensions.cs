using PaymentGateway.Api.Clients.BankApiClient;
using PaymentGateway.Api.Constants;
using PaymentGateway.Core;
using PaymentGateway.Infrastructure.Storage;

namespace PaymentGateway.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IBankApiClient, BankApiClient>();

        var baseAddress = configuration.GetValue<string>("BankApi:BaseUrl") 
                          ?? throw new ArgumentException("BankApi:BaseUrl not set!!!");
        
        services.AddHttpClient(ClientNames.BankApiClient, client =>
        {
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }).AddStandardResilienceHandler();
        
        return services;
    }
    
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentsRepository, PaymentsRepository>();

        return services;
    }
}