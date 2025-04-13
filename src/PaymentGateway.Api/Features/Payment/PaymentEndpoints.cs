using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Common;
using PaymentGateway.Api.Features.Payment.Contracts;
using PaymentGateway.Api.Ports;

namespace PaymentGateway.Api.Features.Payment;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/payments/{id:guid}", GetPaymentDetails)
            .WithName("GetPaymentDetails")
            .Produces<GetPaymentResponse>(StatusCodes.Status200OK);
        
        app.MapPost("/api/payments", MakePayment)
            .WithName("MakePayment")
            .Accepts<PostPaymentRequest>("application/json")
            .Produces<PostPaymentResponse>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> GetPaymentDetails(
        [FromRoute] Guid id,
        [FromServices] IPaymentService paymentService)
    {
        var result = await paymentService.GetPayment(id);
        
        if (result.Status is Status.Success)
        {
            return Results.Ok(new GetPaymentResponse
            {
                Id = id,
                ExpiryMonth = result.Data!.ExpiryMonth,
                ExpiryYear = result.Data.ExpiryYear,
                Currency = result.Data.Currency,
                Amount = result.Data.Amount,
                CardNumberLastFour = result.Data.CardNumber[^4..],
                Status = Enum.Parse<PaymentStatus>(result.Data!.Status.ToString()),
            });
        }

        return result.Status switch
        {
            Status.Error => Results.Problem(result.Message, statusCode: StatusCodes.Status500InternalServerError),
            _ => Results.Problem(result.Message, statusCode: StatusCodes.Status500InternalServerError)
        };
    }
    
    private static async Task<IResult> MakePayment(
        [FromBody] PostPaymentRequest request,
        [FromServices] IPaymentService paymentService)
    {
        var payment = new Domain.Models.Payment
        {
            Amount = request.Amount,
            ExpiryMonth = request.ExpiryMonth,
            ExpiryYear = request.ExpiryYear,
            Currency = request.Currency,
            CardNumber = request.CardNumber,
            Cvv = request.Cvv,
        };
        
        var result = await paymentService.MakePayment(payment);

        if (result.Status is Status.Success)
        {
            return Results.Ok(new PostPaymentResponse
            {
                Id = result.Data!.Id,
                Status = PaymentStatus.Authorized
            });
        }

        return result.Status switch
        {
            Status.Error => Results.Problem(result.Message, statusCode: StatusCodes.Status500InternalServerError),
            _ => Results.Problem(result.Message, statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}