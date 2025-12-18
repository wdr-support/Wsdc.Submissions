using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using Wsdc.Submissions.Apps.RestAPI.Swagger;
using Wsdc.Submissions.Core;
using Wsdc.Submissions.Models.Dtos;
using Wsdc.Submissions.Services.Abstract;

namespace Wsdc.Submissions.Apps.RestAPI.Controllers;

/// <summary>
/// Controller for WSDC event data submissions and validation
/// </summary>
[ApiController]
[Route("api/results")]
public class SubmissionsController : ControllerBase
{
    private readonly IWsdcSubmissionsService _submissionsService;
    private readonly IWsdcSubmissionsValidationService _validationService;
    private readonly ILogger<SubmissionsController> _logger;

    public SubmissionsController(
        IWsdcSubmissionsService submissionsService,
        IWsdcSubmissionsValidationService validationService,
        ILogger<SubmissionsController> logger)
    {
        _submissionsService = submissionsService ?? throw new ArgumentNullException(nameof(submissionsService));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates event data using FluentValidation
    /// </summary>
    /// <param name="request">Event submission request containing event data</param>
    /// <returns>Validation result with summary data on success, or errors on failure</returns>
    /// <response code="200">Validation successful with summary of event data</response>
    /// <response code="400">Invalid request or validation failed</response>
    [HttpPost("validate")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ServiceResponse<SubmissionResultResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<SubmissionResultResponse>), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(EventResultsRequest), typeof(EventSubmissionRequestExample))]
    public async Task<IActionResult> Validate([FromBody] EventResultsRequest request)
    {
        _logger.LogInformation("Received validation request");

        var result = await _validationService.ValidateEventDataAsync(request);

        if (result.HasErrors)
        {
            _logger.LogWarning("Validation failed with {ErrorCount} errors", result.Errors.Count);
            return BadRequest(result);
        }

        _logger.LogInformation("Validation successful");
        return Ok(result);
    }

    /// <summary>
    /// Submits event data after validation
    /// </summary>
    /// <param name="request">Event submission request containing event data</param>
    /// <returns>Submission result with summary data on success, or errors on failure</returns>
    /// <response code="200">Submission completed successfully with summary of submitted data</response>
    /// <response code="400">Invalid request or validation failed</response>
    [HttpPost("submit")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ServiceResponse<SubmissionResultResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResponse<SubmissionResultResponse>), StatusCodes.Status400BadRequest)]
    [SwaggerRequestExample(typeof(EventResultsRequest), typeof(EventSubmissionRequestExample))]
    public async Task<IActionResult> Submit([FromBody] EventResultsRequest request)
    {
        _logger.LogInformation("Received submission request");

        var result = await _submissionsService.SubmitEventDataAsync(request);

        if (result.HasErrors)
        {
            _logger.LogWarning("Submission failed with {ErrorCount} errors", result.Errors.Count);
            return BadRequest(result);
        }

        _logger.LogInformation("Submission successful");
        return Ok(result);
    }
}

