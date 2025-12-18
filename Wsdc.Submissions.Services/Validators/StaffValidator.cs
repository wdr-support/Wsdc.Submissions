using FluentValidation;
using Wsdc.Submissions.Models.Dtos;
using Wsdc.Submissions.Models.Enums;
using Wsdc.Submissions.Services.Validators.Constants;

namespace Wsdc.Submissions.Services.Validators;

/// <summary>
/// Validator for Staff DTO
/// </summary>
public class StaffValidator : AbstractValidator<Staff>
{
    public StaffValidator()
    {
        // STAFF ID VALIDATION
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage("Staff ID is required")
            .WithErrorCode("STAFF_ID_REQUIRED");

        RuleFor(x => x.Id)
            .MaximumLength(ValidationConstants.MaxIdLength)
            .WithMessage($"Staff ID must not exceed {ValidationConstants.MaxIdLength} characters")
            .WithErrorCode("STAFF_ID_TOO_LONG");

        RuleFor(x => x.Id)
            .Must(BeValidIdFormat)
            .WithMessage("Staff ID must be a positive integer or blank")
            .WithErrorCode("INVALID_STAFF_ID_FORMAT");

        // STAFF TYPE VALIDATION
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Staff type must be one of: EVENT_DIRECTOR, CHIEF_JUDGE_PRIMARY, CHIEF_JUDGE_SECONDARY, TABULATOR, SCORING_PERSON, DECK_CAPTAIN, MUSIC_DIRECTOR")
            .WithErrorCode("INVALID_STAFF_TYPE");

        // STAFF NAME VALIDATION
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Staff name is required")
            .WithErrorCode("STAFF_NAME_REQUIRED");

        RuleFor(x => x.Name)
            .MinimumLength(1)
            .WithMessage("Staff name must be at least 1 character")
            .WithErrorCode("STAFF_NAME_TOO_SHORT");

        RuleFor(x => x.Name)
            .MaximumLength(ValidationConstants.MaxNameLength)
            .WithMessage($"Staff name must not exceed {ValidationConstants.MaxNameLength} characters")
            .WithErrorCode("STAFF_NAME_TOO_LONG");

        // STAFF EMAIL VALIDATION
        RuleFor(x => x.Email)
            .MaximumLength(ValidationConstants.MaxEmailLength)
            .WithMessage($"Staff email must not exceed {ValidationConstants.MaxEmailLength} characters")
            .WithErrorCode("STAFF_EMAIL_TOO_LONG");

        // STAFF PHONE VALIDATION
        RuleFor(x => x.Phone)
            .MaximumLength(ValidationConstants.MaxPhoneLength)
            .WithMessage($"Staff phone must not exceed {ValidationConstants.MaxPhoneLength} characters")
            .WithErrorCode("STAFF_PHONE_TOO_LONG");
    }

    /// <summary>
    /// Validates that staff ID is a positive integer or blank
    /// </summary>
    private bool BeValidIdFormat(string id)
    {
        if (string.IsNullOrEmpty(id)) return true;
        return int.TryParse(id, out var intId) && intId > 0;
    }
}

