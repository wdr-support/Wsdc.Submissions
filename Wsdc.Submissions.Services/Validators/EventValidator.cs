using FluentValidation;
using Wsdc.Submissions.Models.Dtos;
using Wsdc.Submissions.Models.Enums;
using Wsdc.Submissions.Services.Validators.Constants;

namespace Wsdc.Submissions.Services.Validators;

/// <summary>
/// Validator for Event DTO with custom business rules
/// </summary>
public class EventValidator : AbstractValidator<Event>
{
    public EventValidator()
    {
        // EVENT NAME VALIDATION
        RuleFor(x => x.EventName)
            .NotEmpty()
            .WithMessage("Event name is required")
            .WithErrorCode("EVENT_NAME_REQUIRED");

        RuleFor(x => x.EventName)
            .MinimumLength(1)
            .WithMessage("Event name must be at least 1 character")
            .WithErrorCode("EVENT_NAME_TOO_SHORT");

        RuleFor(x => x.EventName)
            .MaximumLength(ValidationConstants.MaxNameLength)
            .WithMessage($"Event name must not exceed {ValidationConstants.MaxNameLength} characters")
            .WithErrorCode("EVENT_NAME_TOO_LONG");

        // EVENT DATE VALIDATION
        RuleFor(x => x.EventStartDate)
            .NotEmpty()
            .WithMessage("Event start date is required")
            .WithErrorCode("EVENT_START_DATE_REQUIRED");

        RuleFor(x => x.EventEndDate)
            .NotEmpty()
            .WithMessage("Event end date is required")
            .WithErrorCode("EVENT_END_DATE_REQUIRED");

        // Custom business rule: End date must be on or after start date
        RuleFor(x => x.EventEndDate)
            .GreaterThanOrEqualTo(x => x.EventStartDate)
            .WithMessage("Event end date must be on or after the start date")
            .WithErrorCode("INVALID_DATE_RANGE");

        // EVENT LOCATION VALIDATION
        RuleFor(x => x.VenueName)
            .NotEmpty()
            .WithMessage("Event location is required")
            .WithErrorCode("EVENT_LOCATION_REQUIRED");

        RuleFor(x => x.VenueName)
            .MaximumLength(ValidationConstants.MaxLocationLength)
            .WithMessage($"Event location must not exceed {ValidationConstants.MaxLocationLength} characters")
            .WithErrorCode("EVENT_LOCATION_TOO_LONG");

        // STAFF VALIDATION
        RuleFor(x => x.Staff)
            .NotNull()
            .WithMessage("Staff list is required")
            .WithErrorCode("STAFF_REQUIRED");

        RuleFor(x => x.Staff)
            .NotEmpty()
            .WithMessage("Event must have at least one staff member")
            .WithErrorCode("NO_STAFF");

        RuleForEach(x => x.Staff)
            .SetValidator(new StaffValidator());

        // Custom business rule: Must have at least one Chief Judge
        RuleFor(x => x.Staff)
            .Must(HaveAtLeastOneChiefJudge)
            .WithMessage("Event must have at least one Chief Judge (CHIEF_JUDGE_PRIMARY or CHIEF_JUDGE_SECONDARY)")
            .WithErrorCode("MISSING_CHIEF_JUDGE");

        // DIVISIONS VALIDATION
        RuleFor(x => x.DivisionsRegistry)
            .NotNull()
            .WithMessage("Divisions list is required")
            .WithErrorCode("DIVISIONS_REQUIRED");

        RuleFor(x => x.DivisionsRegistry)
            .NotEmpty()
            .WithMessage("Event must have at least one division")
            .WithErrorCode("NO_DIVISIONS");

        RuleForEach(x => x.DivisionsRegistry)
            .SetValidator(new DivisionValidator());
    }

    /// <summary>
    /// Validates that the event has at least one Chief Judge
    /// </summary>
    private bool HaveAtLeastOneChiefJudge(List<Staff> staff)
    {
        if (staff == null || staff.Count == 0) return false;

        return staff.Any(s =>
            s.Type == StaffType.CHIEF_JUDGE_PRIMARY);
    }
}

