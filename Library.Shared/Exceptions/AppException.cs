using System.Net;

namespace Library.Shared.Exceptions;

public class AppException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
    : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;

    public static AppException NotFound(string message) 
        => new(message, HttpStatusCode.NotFound);

    public static AppException Conflict(string message) 
        => new(message, HttpStatusCode.Conflict);

    public static AppException BadRequest(string message) 
        => new(message, HttpStatusCode.BadRequest);

    public static AppException Forbidden(string message) 
        => new(message, HttpStatusCode.Forbidden);
}