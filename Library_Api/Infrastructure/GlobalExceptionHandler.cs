using System.Net;
using Library_Api.Common;
using Library.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Library_Api.Infrastructure;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, message) = exception switch
        {
            AppException appEx => (appEx.StatusCode, appEx.Message),
            ArgumentException argEx => (HttpStatusCode.BadRequest, argEx.Message),
            KeyNotFoundException notFoundEx => (HttpStatusCode.NotFound, notFoundEx.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
        };

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        var errors = statusCode == HttpStatusCode.InternalServerError
            ? null
            : new List<string> { message };

        var response = ApiResponse.Failure(
            message: message,
            errors: errors
        );

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}