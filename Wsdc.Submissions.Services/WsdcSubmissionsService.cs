using FluentValidation;
using Microsoft.Extensions.Logging;
using Wsdc.Submissions.Core;
using Wsdc.Submissions.Models.Dtos;
using Wsdc.Submissions.Services.Abstract;
using Wsdc.Submissions.Services.Validators.Constants;
using Wsdc.Submissions.Services.Validators.Helpers;

namespace Wsdc.Submissions.Services;

/// <summary>
/// Service for submitting WSDC event data
/// </summary>
public class WsdcSubmissionsService : IWsdcSubmissionsService
{
    private readonly IValidator<EventResultsRequest> _validator;
    private readonly ISubmissionResultBuilder _resultBuilder;
    private readonly ILogger<WsdcSubmissionsService> _logger;

    public WsdcSubmissionsService(
        IValidator<EventResultsRequest> validator,
        ISubmissionResultBuilder resultBuilder,
        ILogger<WsdcSubmissionsService> logger)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _resultBuilder = resultBuilder ?? throw new ArgumentNullException(nameof(resultBuilder));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<ServiceResponse<SubmissionResultResponse>> SubmitEventDataAsync(EventResultsRequest request)
    {
        _logger.LogInformation("Starting event data submission");

        // Validate input
        if (request == null)
        {
            _logger.LogWarning("Submission request received with null payload");
            return ServiceResponse<SubmissionResultResponse>.Failure(
                ValidationConstants.ErrorCodeValidationError,
                "Request cannot be null",
                "$",
                ErrorSeverity.Error
            );
        }

        // Perform validation
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Event data submission failed validation with {ErrorCount} errors", validationResult.Errors.Count);

            var serviceErrors = ValidationErrorConverter.ToServiceErrors(validationResult.Errors);

            return ServiceResponse<SubmissionResultResponse>.Failure(serviceErrors);
        }

        _logger.LogInformation("Event data submission validation successful");

        // TODO: do something with the validated event results
        // ensure this operation is idempotent

        // Build and return the submission result response
        var response = _resultBuilder.Build(request);

        _logger.LogInformation(
            "Submission completed for {DivisionCount} divisions at {SubmittedAt}",
            response.Divisions.Count,
            response.SubmittedAt);

        return ServiceResponse<SubmissionResultResponse>.Success(response);
    }
}

