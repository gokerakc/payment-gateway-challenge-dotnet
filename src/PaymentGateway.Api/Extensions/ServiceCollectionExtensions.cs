using PaymentGateway.Api.Constants;
using PaymentGateway.Api.Domain.Services;
using PaymentGateway.Api.Features.Payment;
using PaymentGateway.Api.Infrastructure.Clients.BankApiClient;
using PaymentGateway.Api.Infrastructure.Storage;
using PaymentGateway.Api.Ports.Clients.BankApiClient;
using PaymentGateway.Api.Ports.Services;
using PaymentGateway.Api.Ports.Storage;

namespace PaymentGateway.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddSingleton<PostPaymentRequestValidator>();
        
        return services;
    }
    
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
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
}