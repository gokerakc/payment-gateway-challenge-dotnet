using System.Text.Json.Serialization;

using PaymentGateway.Api.Extensions;
using PaymentGateway.Api.Features.Payment;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

// Add domain specific dependencies (payment services, payment validators, etc.)
builder.Services.AddDomainServices();

// Add infra specific dependencies (databases, clients, messaging, etc.)
builder.Services.AddInfrastructureServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapPaymentEndpoints();

app.Run();

public partial class Program;

// TODO IDEAS:
// add serilog, use structured logging
// use global error handling
// use polly retries
// introduce idempotency key