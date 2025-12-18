using FluentValidation;
using Wsdc.Submissions.Models.Dtos;
using Wsdc.Submissions.Services.Validators.Constants;

namespace Wsdc.Submissions.Services.Validators;

/// <summary>
/// Validator for Judge DTO
/// </summary>
public class JudgeValidator : AbstractValidator<Judge>
{
    public JudgeValidator()
    {
        // JUDGE ID VALIDATION
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage("Judge ID is required")
            .WithErrorCode("JUDGE_ID_REQUIRED");

        RuleFor(x => x.Id)
            .MaximumLength(ValidationConstants.MaxIdLength)
            .WithMessage($"Judge ID must not exceed {ValidationConstants.MaxIdLength} characters")
            .WithErrorCode("JUDGE_ID_TOO_LONG");

        RuleFor(x => x.Id)
            .Must(BeValidIdFormat)
            .WithMessage("Judge ID must be a positive integer or blank")
            .WithErrorCode("INVALID_JUDGE_ID_FORMAT");

        // JUDGE SCORE VALIDATION
        RuleFor(x => x.Score)
            .NotNull()
            .WithMessage("Judge score is required")
            .WithErrorCode("JUDGE_SCORE_REQUIRED");

        RuleFor(x => x.Score)
            .MaximumLength(ValidationConstants.MaxScoreLength)
            .WithMessage($"Judge score must not exceed {ValidationConstants.MaxScoreLength} characters")
            .WithErrorCode("JUDGE_SCORE_TOO_LONG");

        // Note: Round-type-specific validation (prelim vs final) is handled in RoundValidator
        // where we have context about the round type
    }

    /// <summary>
    /// Validates that judge ID is a positive integer or blank
    /// </summary>
    private bool BeValidIdFormat(string id)
    {
        if (string.IsNullOrEmpty(id)) return true;
        return int.TryParse(id, out var intId) && intId > 0;
    }
}

