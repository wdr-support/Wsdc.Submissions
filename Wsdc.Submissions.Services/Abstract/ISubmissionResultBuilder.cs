using Wsdc.Submissions.Models.Dtos;

namespace Wsdc.Submissions.Services.Abstract;

/// <summary>
/// Service for building submission result responses from event data
/// </summary>
public interface ISubmissionResultBuilder
{
    /// <summary>
    /// Builds a submission result response from the event results request
    /// </summary>
    /// <param name="request">The event results request that was submitted</param>
    /// <returns>A populated SubmissionResultResponse with summary information</returns>
    SubmissionResultResponse Build(EventResultsRequest request);
}

