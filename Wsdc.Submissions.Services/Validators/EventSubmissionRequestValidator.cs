using FluentValidation;
using Wsdc.Submissions.Models.Dtos;

namespace Wsdc.Submissions.Services.Validators;

/// <summary>
/// Validator for EventSubmissionRequest DTO (root object)
/// </summary>
public class EventSubmissionRequestValidator : AbstractValidator<EventResultsRequest>
{
    public EventSubmissionRequestValidator()
    {
        // EVENT VALIDATION
        RuleFor(x => x.Event)
            .NotNull()
            .WithMessage("Event data is required")
            .WithErrorCode("EVENT_REQUIRED");

        RuleFor(x => x.Event)
            .SetValidator(new EventValidator());
    }
}

