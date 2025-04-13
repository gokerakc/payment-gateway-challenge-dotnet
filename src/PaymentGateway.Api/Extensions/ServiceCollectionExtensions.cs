using PaymentGateway.Api.Constants;
using PaymentGateway.Api.Domain.Services;
using PaymentGateway.Api.Infrastructure.Clients.BankApiClient;
using PaymentGateway.Api.Infrastructure.Storage;
using PaymentGateway.Api.Ports;
using PaymentGateway.Api.Ports.Clients;

namespace PaymentGateway.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IPaymentValidator, PaymentValidator>();
        
        return services;
    }
    
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
        services.AddSingleton<IBankApiClient, BankApiClient>();

        services.AddHttpClient(ClientNames.BankApiClient, client =>
        {
            client.BaseAddress = new Uri("http://localhost:8080/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        

        return services;
    }
}