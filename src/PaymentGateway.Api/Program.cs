using System.Text.Json.Serialization;

using PaymentGateway.Api.Endpoints.CreatePayment;
using PaymentGateway.Api.Endpoints.GetPayment;
using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Extensions;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Host.UseSerilog((_, services, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssemblies(typeof(Program).Assembly); 
});
builder.Services.AddInfrastructureServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGetPaymentEndpoints();
app.MapCreatePaymentEndpoints();

app.Run();

public partial class Program;