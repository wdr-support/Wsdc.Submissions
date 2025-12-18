using Wsdc.Submissions.Core;
using Wsdc.Submissions.Models.Dtos;

namespace Wsdc.Submissions.Services.Abstract;

/// <summary>
/// Service for validating WSDC event data submissions
/// </summary>
public interface IWsdcSubmissionsValidationService
{
    /// <summary>
    /// Validates event data and returns a summary if valid
    /// </summary>
    /// <param name="request">The event submission request to validate</param>
    /// <returns>ServiceResponse containing submission summary on success, or validation errors on failure</returns>
    Task<ServiceResponse<SubmissionResultResponse>> ValidateEventDataAsync(EventResultsRequest request);
}

