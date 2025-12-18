namespace Wsdc.Submissions.Core;

/// <summary>
/// Generic response wrapper for service operations
/// </summary>
/// <typeparam name="T">Type of the response data payload</typeparam>
public class ServiceResponse<T>
{
    /// <summary>
    /// The response data payload (null when HasErrors is true)
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Indicates whether the operation encountered errors
    /// </summary>
    public bool HasErrors { get; set; }

    /// <summary>
    /// Collection of errors that occurred during the operation
    /// </summary>
    public List<ServiceError> Errors { get; set; } = new();

    /// <summary>
    /// Creates a successful response with data
    /// </summary>
    public static ServiceResponse<T> Success(T data)
    {
        return new ServiceResponse<T>
        {
            Data = data,
            HasErrors = false,
            Errors = new List<ServiceError>()
        };
    }

    /// <summary>
    /// Creates a failed response with errors
    /// </summary>
    public static ServiceResponse<T> Failure(List<ServiceError> errors)
    {
        return new ServiceResponse<T>
        {
            Data = default,
            HasErrors = true,
            Errors = errors
        };
    }

    /// <summary>
    /// Creates a failed response with a single error
    /// </summary>
    public static ServiceResponse<T> Failure(ServiceError error)
    {
        return new ServiceResponse<T>
        {
            Data = default,
            HasErrors = true,
            Errors = new List<ServiceError> { error }
        };
    }

    /// <summary>
    /// Creates a failed response with error details
    /// </summary>
    public static ServiceResponse<T> Failure(string errorCode, string message, string? propertyPath = null, ErrorSeverity severity = ErrorSeverity.Error)
    {
        return new ServiceResponse<T>
        {
            Data = default,
            HasErrors = true,
            Errors = new List<ServiceError>
            {
                new ServiceError(errorCode, message, propertyPath, severity)
            }
        };
    }
}

