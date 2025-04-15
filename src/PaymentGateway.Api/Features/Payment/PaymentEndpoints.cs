using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Common;
using PaymentGateway.Api.Features.Payment.Contracts;
using PaymentGateway.Api.Ports.Services;

using Serilog;

namespace PaymentGateway.Api.Features.Payment;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/payments/{id:guid}", GetPayment)
            .WithName("GetPayment")
            .Produces<GetPaymentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapPost("/api/payments", MakePayment)
            .WithName("MakePayment")
            .Accepts<PostPaymentRequest>("application/json")
            .Produces<PostPaymentResponse>(StatusCodes.Status200OK)
            .Produces<PostPaymentResponse>(StatusCodes.Status409Conflict)
            .ProducesValidationProblem(statusCode: StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);
    }

    private static async Task<IResult> GetPayment(
        [FromRoute] Guid id,
        [FromHeader(Name = "x-merchant-id")] string? merchantId,
        [FromServices] IPaymentService paymentService,
        [FromServices] IDiagnosticContext diagnosticContext)
    {
        diagnosticContext.Set("PaymentId", id.ToString());
        diagnosticContext.Set("MerchantId", merchantId ?? string.Empty);

        var result = await paymentService.GetPayment(id);
        
        return result.Status switch
        {
            Status.Success => Results.Ok(MapToGetPaymentResponse(result.Data!)),
            Status.NotFound => Results.Problem(title: result.Message, statusCode: StatusCodes.Status404NotFound),
            _ => Results.Problem("Internal server error", statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    private static async Task<IResult> MakePayment(
        [FromBody] PostPaymentRequest request,
        [FromHeader(Name = "x-idempotency-token")] Guid? idempotencyToken,
        [FromHeader(Name = "x-merchant-id")] string? merchantId,
        [FromServices] IPaymentService paymentService,
        [FromServices] PostPaymentRequestValidator validator,
        [FromServices] IDiagnosticContext diagnosticContext)
    {
        var validationResult = await validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.Errors, title: "Payment rejected");
        }

        var paymentId = idempotencyToken ?? Guid.NewGuid();
        diagnosticContext.Set("PaymentId", paymentId.ToString());
        diagnosticContext.Set("MerchantId", merchantId ?? string.Empty);

        var result = await paymentService.MakePayment(new Domain.Models.Payment 
        {
            Id = idempotencyToken ?? Guid.NewGuid(),
            Amount = request.Amount,
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency,
            CardNumber = request.CardNumber,
            Cvv = request.Cvv
        });

        return result.Status switch
        {
            Status.Success or Status.Unauthorized => Results.Ok(MapToPostPaymentResponse(result.Data!)),
            Status.Conflict => Results.Conflict(MapToPostPaymentResponse(result.Data!)),
            Status.Error => Results.Problem(result.Message, statusCode: StatusCodes.Status503ServiceUnavailable),
            _ => Results.Problem("Internal server error", statusCode: StatusCodes.Status500InternalServerError)
        };
    }
    
    private static GetPaymentResponse MapToGetPaymentResponse(Domain.Models.Payment payment)
    {
        return new GetPaymentResponse
        {
            Id = payment.Id,
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount,
            CardNumberLastFour = payment.CardNumber[^4..],
            Status = Enum.Parse<PaymentStatus>(payment!.Status.ToString())
        };
    }

    private static PostPaymentResponse MapToPostPaymentResponse(Domain.Models.Payment payment)
    {
        return new PostPaymentResponse
        {
            Id = payment.Id,
            Status = Enum.Parse<PaymentStatus>(payment.Status.ToString()),
            CardNumberLastFour = payment.CardNumber[^4..],
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount
        };
    }
}