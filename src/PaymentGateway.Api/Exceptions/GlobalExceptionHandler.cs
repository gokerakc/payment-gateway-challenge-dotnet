using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PaymentGateway.Api.Exceptions;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly Serilog.ILogger _logger = Serilog.Log.ForContext<GlobalExceptionHandler>();

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.Error(
            exception, "Exception occurred: {Message}", exception.Message);

        var problemDetails = exception switch
        {
            BadHttpRequestException badHttpRequestException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid payload",
                Detail = badHttpRequestException.InnerException?.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server error",
                Detail = "An unexpected error occurred"
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}