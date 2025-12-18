namespace Wsdc.Submissions.Core;

/// <summary>
/// Represents the severity level of an error or validation issue
/// </summary>
public enum ErrorSeverity
{
    /// <summary>
    /// Informational message
    /// </summary>
    Info = 0,

    /// <summary>
    /// Warning that doesn't prevent processing
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error that prevents successful processing
    /// </summary>
    Error = 2,

    /// <summary>
    /// Critical error requiring immediate attention
    /// </summary>
    Critical = 3
}

