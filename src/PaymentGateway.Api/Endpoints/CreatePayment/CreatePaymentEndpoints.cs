using MediatR;
using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Endpoints.CreatePayment.Contracts;
using PaymentGateway.Application.Common;
using PaymentGateway.Application.Features.Payment.CreatePayment;
using PaymentGateway.Core;
using PaymentGateway.Core.Models;
using Serilog;

namespace PaymentGateway.Api.Endpoints.CreatePayment;

public static class CreatePaymentEndpoints
{
    public static void MapCreatePaymentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/payments", CreatePayment)
            .WithName("CreatePayment")
            .Accepts<PostPaymentRequest>("application/json")
            .Produces<PostPaymentResponse>(StatusCodes.Status200OK)
            .Produces<PostPaymentResponse>(StatusCodes.Status409Conflict)
            .ProducesValidationProblem(statusCode: StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);
    }

    private static async Task<IResult> CreatePayment(
        [FromBody] PostPaymentRequest request,
        [FromHeader(Name = "x-idempotency-token")]
        Guid? idempotencyToken,
        [FromHeader(Name = "x-merchant-id")] string? merchantId,
        [FromServices] IMediator mediator,
        [FromServices] IDiagnosticContext diagnosticContext)
    {
        var paymentId = idempotencyToken ?? Guid.NewGuid();
        diagnosticContext.Set("PaymentId", paymentId.ToString());
        diagnosticContext.Set("MerchantId", merchantId ?? string.Empty);

        var command = new CreatePaymentCommand(
            paymentId: paymentId,
            amount: request.Amount,
            expiryMonth: request.ExpiryMonth,
            expiryYear: request.ExpiryYear,
            currency: request.Currency,
            cardNumber: request.CardNumber,
            cvv: request.Cvv
        );

        var response = await mediator.Send(command);

        return response.Status switch
        {
            Status.Success or Status.Unauthorized => Results.Ok(MapToPostPaymentResponse(response.Data!)),
            Status.Conflict => Results.Conflict(MapToPostPaymentResponse(response.Data!)),
            Status.Invalid => Results.ValidationProblem(response.Errors!, title: "Payment rejected"),
            Status.Error => Results.Problem(response.Message, statusCode: StatusCodes.Status503ServiceUnavailable),
            _ => Results.Problem("Internal server error", statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    private static PostPaymentResponse MapToPostPaymentResponse(Payment payment)
    {
        return new PostPaymentResponse
        {
            Id = payment.Id,
            Status = Enum.Parse<Shared.Contract.PaymentStatus>(payment.Status.ToString()),
            CardNumberLastFour = payment.CardNumber[^4..],
            ExpiryMonth = payment.ExpiryMonth,
            ExpiryYear = payment.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount
        };
    }
}