namespace Library_Api.Common;

public class ApiResponse<T>
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public List<string>? Errors { get; init; }

    // Factory methods للإنشاء السهل والمنظم
    public static ApiResponse<T> Success(T data, string message = "Operation completed successfully")
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            Errors = null
        };
    }

    public static ApiResponse<T> Failure(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Data = default,
            Errors = errors ?? new List<string>()
        };
    }
}

// كلاس غير Generic للعمليات التي لا ترجع بيانات (مثل الـ Update أو Delete)
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Success(string message = "Operation completed successfully")
    {
        return new ApiResponse
        {
            IsSuccess = true,
            Message = message,
            Data = null,
            Errors = null
        };
    }

    public static new ApiResponse Failure(string message, List<string>? errors = null)
    {
        return new ApiResponse
        {
            IsSuccess = false,
            Message = message,
            Data = null,
            Errors = errors ?? new List<string>()
        };
    }
}