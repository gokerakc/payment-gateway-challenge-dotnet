using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api.Infrastructure.Storage;
using PaymentGateway.Api.Ports.Clients.BankApiClient;
using PaymentGateway.Api.Ports.Storage;
using PaymentGateway.IntegrationTests.Mocks;

namespace PaymentGateway.IntegrationTests;

public class ApplicationFixture : WebApplicationFactory<Program>
{
    public readonly PaymentsRepository PaymentsRepository = new PaymentsRepository();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IPaymentsRepository>(_ => PaymentsRepository);
            services.AddSingleton<IBankApiClient>(_ => new MockBankApiClient());
        });
    }
}