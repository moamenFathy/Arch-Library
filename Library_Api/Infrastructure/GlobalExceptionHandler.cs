using System.Net;
using Library_Api.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace Library_Api.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
  private readonly ILogger<GlobalExceptionHandler> _logger;

  public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
  {
    _logger = logger;
  }

  public async ValueTask<bool> TryHandleAsync(
    HttpContext httpContext,
    Exception exception,
    CancellationToken cancellationToken)
  {
    _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
  
    // تحديد كود الـ HTTP Status ورسالة الخطأ بناءً على نوع الـ Exception
    var (statusCode, message) = exception switch
    {
      ArgumentException argEx => (HttpStatusCode.BadRequest, argEx.Message),
      KeyNotFoundException notFoundEx => (HttpStatusCode.NotFound, notFoundEx.Message),
      _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.")
    };
  
    httpContext.Response.StatusCode = (int)statusCode;
    httpContext.Response.ContentType = "application/json";
  
    var response = ApiResponse.Failure(
      message: message,
      errors: new List<string> { exception.Message }
    );
  
    await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
  
    // إرجاع true يعني أن الـ Exception تم التعامل معه بنجاح ولا داعي لتمريره للأعلى
    return true;
  }
}