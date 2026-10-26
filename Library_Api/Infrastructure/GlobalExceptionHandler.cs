using System.Net;
using FluentValidation;
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

        var (statusCode, message, errors) = exception switch
        {
            ValidationException valEx => (
                HttpStatusCode.BadRequest, 
                "Validation failed.", 
                valEx.Errors.Select(e => e.ErrorMessage).Distinct().ToList()
            ),
            AppException appEx => (
                appEx.StatusCode, 
                appEx.Message, 
                new List<string> { appEx.Message }
            ),
            ArgumentException argEx => (
                HttpStatusCode.BadRequest, 
                argEx.Message, 
                new List<string> { argEx.Message }
            ),
            KeyNotFoundException notFoundEx => (
                HttpStatusCode.NotFound, 
                notFoundEx.Message, 
                new List<string> { notFoundEx.Message }
            ),
            _ => (
                HttpStatusCode.InternalServerError, 
                "An unexpected error occurred. Please try again later.", 
                (List<string>?)null
            )
        };

        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        var response = ApiResponse.Failure(
            message: message,
            errors: errors
        );

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
