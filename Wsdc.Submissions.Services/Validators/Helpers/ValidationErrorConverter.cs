using FluentValidation.Results;
using Wsdc.Submissions.Core;

namespace Wsdc.Submissions.Services.Validators.Helpers;

/// <summary>
/// Helper class to convert validation errors to ServiceError format
/// </summary>
public static class ValidationErrorConverter
{
    /// <summary>
    /// Converts ValidationFailure to ServiceError
    /// </summary>
    /// <param name="failure">The validation failure</param>
    /// <returns>ServiceError instance</returns>
    public static ServiceError ToServiceError(ValidationFailure failure)
    {
        return new ServiceError
        {
            ErrorCode = failure.ErrorCode,
            Message = failure.ErrorMessage,
            PropertyPath = failure.PropertyName,
            Severity = MapSeverity(failure.Severity)
        };
    }

    /// <summary>
    /// Converts a list of validation failures to ServiceError list
    /// </summary>
    /// <param name="failures">List of validation failures</param>
    /// <returns>List of ServiceError instances</returns>
    public static List<ServiceError> ToServiceErrors(IEnumerable<ValidationFailure> failures)
    {
        return failures.Select(ToServiceError).ToList();
    }

    /// <summary>
    /// Maps validation Severity to our ErrorSeverity enum
    /// </summary>
    private static ErrorSeverity MapSeverity(FluentValidation.Severity severity)
    {
        return severity switch
        {
            FluentValidation.Severity.Error => ErrorSeverity.Error,
            FluentValidation.Severity.Warning => ErrorSeverity.Warning,
            FluentValidation.Severity.Info => ErrorSeverity.Info,
            _ => ErrorSeverity.Error
        };
    }
}

