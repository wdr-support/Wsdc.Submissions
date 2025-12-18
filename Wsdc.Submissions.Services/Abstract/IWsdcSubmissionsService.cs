using Wsdc.Submissions.Core;
using Wsdc.Submissions.Models.Dtos;

namespace Wsdc.Submissions.Services.Abstract;

/// <summary>
/// Service for submitting WSDC event data
/// </summary>
public interface IWsdcSubmissionsService
{
    /// <summary>
    /// Submits event data after validation
    /// </summary>
    /// <param name="request">The event submission request to submit</param>
    /// <returns>ServiceResponse containing submission result summary on success, or errors on failure</returns>
    Task<ServiceResponse<SubmissionResultResponse>> SubmitEventDataAsync(EventResultsRequest request);
}

