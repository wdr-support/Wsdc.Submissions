using FluentValidation;
using Wsdc.Submissions.Models.Dtos;

namespace Wsdc.Submissions.Services.Validators;

/// <summary>
/// Validator for Competitor DTO
/// </summary>
public class CompetitorValidator : AbstractValidator<Competitor>
{
    public CompetitorValidator()
    {
        // PARTICIPANTS VALIDATION
        RuleFor(x => x.Participants)
            .NotNull()
            .WithMessage("Participants list is required")
            .WithErrorCode("PARTICIPANTS_REQUIRED");

        RuleFor(x => x.Participants)
            .NotEmpty()
            .WithMessage("Competitor must have at least one participant")
            .WithErrorCode("NO_PARTICIPANTS");

        RuleForEach(x => x.Participants)
            .SetValidator(new ParticipantValidator());

        // Custom validation: No duplicate participant IDs
        RuleFor(x => x.Participants)
            .Must(HaveUniqueParticipantIds)
            .WithMessage("Competitor cannot have duplicate participant IDs")
            .WithErrorCode("DUPLICATE_PARTICIPANT_IDS");

        // CALLBACK VALIDATION (optional field, but if present must be a valid enum value)
        When(x => x.Callback.HasValue, () =>
        {
            RuleFor(x => x.Callback)
                .IsInEnum()
                .WithMessage("Callback must be 'Yes', 'No', 'Alt1', 'Alt2', or 'Alt3'")
                .WithErrorCode("INVALID_CALLBACK");
        });

        // JUDGES VALIDATION
        RuleFor(x => x.Judges)
            .NotNull()
            .WithMessage("Judges list is required")
            .WithErrorCode("JUDGES_REQUIRED");

        RuleForEach(x => x.Judges)
            .SetValidator(new JudgeValidator());

        // Custom validation: No duplicate judge IDs
        RuleFor(x => x.Judges)
            .Must(HaveUniqueJudgeIds)
            .WithMessage("Competitor cannot have duplicate judge IDs")
            .WithErrorCode("DUPLICATE_JUDGE_IDS");
    }

    /// <summary>
    /// Validates that there are no duplicate participant IDs (excluding empty)
    /// </summary>
    private bool HaveUniqueParticipantIds(List<Participant> participants)
    {
        if (participants == null || participants.Count == 0) return true;

        var validIds = participants
            .Where(p => !string.IsNullOrEmpty(p.Id))
            .Select(p => p.Id)
            .ToList();

        return validIds.Count == validIds.Distinct().Count();
    }

    /// <summary>
    /// Validates that there are no duplicate judge IDs
    /// </summary>
    private bool HaveUniqueJudgeIds(List<Judge> judges)
    {
        if (judges == null || judges.Count == 0) return true;

        var judgeIds = judges.Select(j => j.Id).ToList();
        return judgeIds.Count == judgeIds.Distinct().Count();
    }
}

