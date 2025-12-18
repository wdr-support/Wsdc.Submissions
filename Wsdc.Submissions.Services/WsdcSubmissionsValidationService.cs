using FluentValidation;
using Microsoft.Extensions.Logging;
using Wsdc.Submissions.Core;
using Wsdc.Submissions.Models.Dtos;
using Wsdc.Submissions.Services.Abstract;
using Wsdc.Submissions.Services.Validators.Constants;
using Wsdc.Submissions.Services.Validators.Helpers;

namespace Wsdc.Submissions.Services;

/// <summary>
/// Service for validating WSDC event data submissions
/// </summary>
public class WsdcSubmissionsValidationService : IWsdcSubmissionsValidationService
{
    private readonly IValidator<EventResultsRequest> _validator;
    private readonly ISubmissionResultBuilder _resultBuilder;
    private readonly ILogger<WsdcSubmissionsValidationService> _logger;

    public WsdcSubmissionsValidationService(
        IValidator<EventResultsRequest> validator,
        ISubmissionResultBuilder resultBuilder,
        ILogger<WsdcSubmissionsValidationService> logger)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _resultBuilder = resultBuilder ?? throw new ArgumentNullException(nameof(resultBuilder));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<ServiceResponse<SubmissionResultResponse>> ValidateEventDataAsync(EventResultsRequest request)
    {
        _logger.LogInformation("Starting event data validation");

        // Validate input
        if (request == null)
        {
            _logger.LogWarning("Validation request received with null payload");
            return ServiceResponse<SubmissionResultResponse>.Failure(
                ValidationConstants.ErrorCodeValidationError,
                "Request cannot be null",
                "$",
                ErrorSeverity.Error
            );
        }

        // Perform validation
        var validationResult = await _validator.ValidateAsync(request);

        if (validationResult.IsValid)
        {
            _logger.LogInformation("Event data validation successful");

            // Build and return the submission result response
            var response = _resultBuilder.Build(request);

            _logger.LogInformation(
                "Validation completed for {DivisionCount} divisions at {SubmittedAt}",
                response.Divisions.Count,
                response.SubmittedAt);

            return ServiceResponse<SubmissionResultResponse>.Success(response);
        }

        // Validation failed - convert errors to service errors
        _logger.LogWarning("Event data validation failed with {ErrorCount} errors", validationResult.Errors.Count);

        var serviceErrors = ValidationErrorConverter.ToServiceErrors(validationResult.Errors);

        return ServiceResponse<SubmissionResultResponse>.Failure(serviceErrors);
    }
}

