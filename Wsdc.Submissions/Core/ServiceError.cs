namespace Wsdc.Submissions.Core;

/// <summary>
/// Represents a single error or validation issue
/// </summary>
public class ServiceError
{
    /// <summary>
    /// Machine-readable error code for programmatic handling
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// JSON path to the problematic field (e.g., "$.event.divisions[0].rounds[1].competitors[2]")
    /// Null or empty if error is not field-specific
    /// </summary>
    public string? PropertyPath { get; set; }

    /// <summary>
    /// Severity level of the error
    /// </summary>
    public ErrorSeverity Severity { get; set; } = ErrorSeverity.Error;

    /// <summary>
    /// Creates a new ServiceError instance
    /// </summary>
    public ServiceError()
    {
    }

    /// <summary>
    /// Creates a new ServiceError with specified values
    /// </summary>
    public ServiceError(string errorCode, string message, string? propertyPath = null, ErrorSeverity severity = ErrorSeverity.Error)
    {
        ErrorCode = errorCode;
        Message = message;
        PropertyPath = propertyPath;
        Severity = severity;
    }
}

