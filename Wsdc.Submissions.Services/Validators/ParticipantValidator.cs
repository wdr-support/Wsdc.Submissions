using FluentValidation;
using Wsdc.Submissions.Models.Dtos;
using Wsdc.Submissions.Models.Enums;
using Wsdc.Submissions.Services.Validators.Constants;

namespace Wsdc.Submissions.Services.Validators;

/// <summary>
/// Validator for Participant DTO
/// </summary>
public class ParticipantValidator : AbstractValidator<Participant>
{
    public ParticipantValidator()
    {
        // PARTICIPANT ID VALIDATION
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage("Participant ID is required")
            .WithErrorCode("PARTICIPANT_ID_REQUIRED");

        RuleFor(x => x.Id)
            .MaximumLength(ValidationConstants.MaxIdLength)
            .WithMessage($"Participant ID must not exceed {ValidationConstants.MaxIdLength} characters")
            .WithErrorCode("PARTICIPANT_ID_TOO_LONG");

        RuleFor(x => x.Id)
            .Must(BeValidIdFormat)
            .WithMessage("Participant ID must be a positive integer or blank")
            .WithErrorCode("INVALID_PARTICIPANT_ID_FORMAT");

        // PARTICIPANT TYPE VALIDATION
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Participant type must be 'Leader' or 'Follower'")
            .WithErrorCode("INVALID_PARTICIPANT_TYPE");

        // PARTICIPANT NAME VALIDATION
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Participant name is required")
            .WithErrorCode("PARTICIPANT_NAME_REQUIRED");

        RuleFor(x => x.Name)
            .MinimumLength(1)
            .WithMessage("Participant name must be at least 1 character")
            .WithErrorCode("PARTICIPANT_NAME_TOO_SHORT");

        RuleFor(x => x.Name)
            .MaximumLength(ValidationConstants.MaxNameLength)
            .WithMessage($"Participant name must not exceed {ValidationConstants.MaxNameLength} characters")
            .WithErrorCode("PARTICIPANT_NAME_TOO_LONG");
    }

    /// <summary>
    /// Validates that participant ID is a positive integer or blank
    /// </summary>
    private bool BeValidIdFormat(string id)
    {
        if (string.IsNullOrEmpty(id)) return true;
        return int.TryParse(id, out var intId) && intId > 0;
    }
}

