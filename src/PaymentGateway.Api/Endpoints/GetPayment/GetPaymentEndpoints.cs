using MediatR;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Common;
using PaymentGateway.Api.Features.Payment.GetPayment;
using PaymentGateway.Core;
using PaymentGateway.Core.Models;

using Serilog;

using GetPaymentResponse = PaymentGateway.Api.Endpoints.GetPayment.Contracts.GetPaymentResponse;
using PaymentStatus = PaymentGateway.Api.Endpoints.Shared.Contract.PaymentStatus;

namespace PaymentGateway.Api.Endpoints.GetPayment;

public static class GetPaymentEndpoints
{
    public static void MapGetPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/payments/{id:guid}", GetPayment)
            .WithName("GetPayment")
            .Produces<GetPaymentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetPayment(
        [FromRoute] Guid id,
        [FromHeader(Name = "x-merchant-id")] string? merchantId,
        [FromServices] IMediator mediator,
        [FromServices] IDiagnosticContext diagnosticContext)
    {
        diagnosticContext.Set("PaymentId", id.ToString());
        diagnosticContext.Set("MerchantId", merchantId ?? string.Empty);

        var response = await mediator.Send(new GetPaymentQuery(id));
        
        return response.Status switch
        {
            Status.Success => Results.Ok(MapToGetPaymentResponse(response.Data!)),
            Status.NotFound => Results.Problem(title: response.Message, statusCode: StatusCodes.Status404NotFound),
            _ => Results.Problem("Internal server error", statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    private static GetPaymentResponse MapToGetPaymentResponse(Core.Models.Payment payment)
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
}