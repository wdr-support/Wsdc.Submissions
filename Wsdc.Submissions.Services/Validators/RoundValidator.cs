using FluentValidation;
using Wsdc.Submissions.Models.Dtos;
using Wsdc.Submissions.Models.Enums;

namespace Wsdc.Submissions.Services.Validators;

/// <summary>
/// Validator for Round DTO with round-type-specific validation
/// </summary>
public class RoundValidator : AbstractValidator<Round>
{
    public RoundValidator()
    {
        // ROUND TYPE VALIDATION
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Round type must be 'Prelim', 'Quarterfinal', 'Semifinal', or 'Final'")
            .WithErrorCode("INVALID_ROUND_TYPE");

        // COMPETITORS VALIDATION
        RuleFor(x => x.Competitors)
            .NotNull()
            .WithMessage("Competitors list is required")
            .WithErrorCode("COMPETITORS_REQUIRED");

        RuleFor(x => x.Competitors)
            .NotEmpty()
            .WithMessage("Round must have at least one competitor")
            .WithErrorCode("NO_COMPETITORS");

        RuleForEach(x => x.Competitors)
            .SetValidator(new CompetitorValidator());

        // ROUND-TYPE-SPECIFIC SCORE VALIDATION
        // Valid scores for non-final rounds
        var validNonFinalScores = new HashSet<string> { "10", "4.5", "4.3", "4.2", "0" };

        // For non-final rounds (Prelims, Quarters, Semis): validate scores, single participant, and callback
        When(x => x.Type == RoundType.Prelims || x.Type == RoundType.Quarters || x.Type == RoundType.Semis, () =>
        {
            RuleFor(x => x.Competitors)
                .Custom((competitors, context) =>
                {
                    ValidateNonFinalScores(competitors, validNonFinalScores, context);
                    ValidateSingleParticipant(competitors, context);
                    ValidateCallbackRequired(competitors, context);
                });
        });

        // For Final rounds: validate scores, participant pairs, and no callback
        When(x => x.Type == RoundType.Finals, () =>
        {
            RuleFor(x => x.Competitors)
                .Custom((competitors, context) =>
                {
                    ValidateFinalScores(competitors, context);
                    ValidateLeaderFollowerPairs(competitors, context);
                    ValidateCallbackNotAllowed(competitors, context);
                });
        });
    }

    /// <summary>
    /// Validates non-final round scores and adds individual errors with specific paths
    /// </summary>
    private void ValidateNonFinalScores(
        List<Competitor> competitors,
        HashSet<string> validScores,
        ValidationContext<Round> context)
    {
        for (var competitorIndex = 0; competitorIndex < competitors.Count; competitorIndex++)
        {
            var competitor = competitors[competitorIndex];
            if (competitor.Judges == null) continue;

            for (var judgeIndex = 0; judgeIndex < competitor.Judges.Count; judgeIndex++)
            {
                var judge = competitor.Judges[judgeIndex];
                var propertyPath = $"Competitors[{competitorIndex}].Judges[{judgeIndex}].Score";

                if (string.IsNullOrEmpty(judge.Score))
                {
                    context.AddFailure(propertyPath, "Score is required");
                }
                else if (!validScores.Contains(judge.Score))
                {
                    context.AddFailure(propertyPath,
                        $"Score '{judge.Score}' is invalid. Must be '10', '4.5', '4.3', '4.2', or '0'");
                }
            }
        }
    }

    /// <summary>
    /// Validates final round scores and adds individual errors with specific paths
    /// </summary>
    private void ValidateFinalScores(List<Competitor> competitors, ValidationContext<Round> context)
    {
        for (var competitorIndex = 0; competitorIndex < competitors.Count; competitorIndex++)
        {
            var competitor = competitors[competitorIndex];
            if (competitor.Judges == null) continue;

            for (var judgeIndex = 0; judgeIndex < competitor.Judges.Count; judgeIndex++)
            {
                var judge = competitor.Judges[judgeIndex];
                var propertyPath = $"Competitors[{competitorIndex}].Judges[{judgeIndex}].Score";

                if (string.IsNullOrEmpty(judge.Score))
                {
                    context.AddFailure(propertyPath, "Score is required");
                }
                else if (!int.TryParse(judge.Score, out var scoreInt))
                {
                    context.AddFailure(propertyPath,
                        $"Score '{judge.Score}' is invalid. Must be a positive integer (1 or greater)");
                }
                else if (scoreInt < 1)
                {
                    context.AddFailure(propertyPath,
                        $"Score '{judge.Score}' is invalid. Must be a positive integer (1 or greater)");
                }
            }
        }
    }

    /// <summary>
    /// Validates that non-final round competitors have exactly 1 participant
    /// </summary>
    private void ValidateSingleParticipant(List<Competitor> competitors, ValidationContext<Round> context)
    {
        for (var competitorIndex = 0; competitorIndex < competitors.Count; competitorIndex++)
        {
            var competitor = competitors[competitorIndex];
            var propertyPath = $"Competitors[{competitorIndex}].Participants";

            if (competitor.Participants == null || competitor.Participants.Count != 1)
            {
                context.AddFailure(propertyPath,
                    "Non-final round competitors must have exactly 1 participant");
            }
            // Note: Participant type validation is handled by ParticipantValidator
        }
    }

    /// <summary>
    /// Validates leader/follower pairs and adds individual errors with specific paths
    /// </summary>
    private void ValidateLeaderFollowerPairs(List<Competitor> competitors, ValidationContext<Round> context)
    {
        for (var competitorIndex = 0; competitorIndex < competitors.Count; competitorIndex++)
        {
            var competitor = competitors[competitorIndex];
            var propertyPath = $"Competitors[{competitorIndex}].Participants";

            if (competitor.Participants == null || competitor.Participants.Count != 2)
            {
                context.AddFailure(propertyPath,
                    "Final round competitors must have exactly 2 participants");
                continue;
            }

            var hasLeader = competitor.Participants.Any(p => p.Type == ParticipantType.Leader);
            var hasFollower = competitor.Participants.Any(p => p.Type == ParticipantType.Follower);

            if (!hasLeader)
            {
                context.AddFailure(propertyPath, "Final round competitor must have one Leader");
            }

            if (!hasFollower)
            {
                context.AddFailure(propertyPath, "Final round competitor must have one Follower");
            }
        }
    }

    /// <summary>
    /// Validates that non-final round competitors have a valid callback value
    /// </summary>
    private void ValidateCallbackRequired(List<Competitor> competitors, ValidationContext<Round> context)
    {
        for (var competitorIndex = 0; competitorIndex < competitors.Count; competitorIndex++)
        {
            var competitor = competitors[competitorIndex];
            var propertyPath = $"Competitors[{competitorIndex}].Callback";

            if (!competitor.Callback.HasValue)
            {
                context.AddFailure(propertyPath,
                    "Callback is required for non-final rounds. Must be 'Yes', 'No', 'Alt1', 'Alt2', or 'Alt3'");
            }
            else if (!Enum.IsDefined(typeof(CallbackType), competitor.Callback.Value))
            {
                context.AddFailure(propertyPath,
                    $"Callback '{competitor.Callback.Value}' is invalid. Must be 'Yes', 'No', 'Alt1', 'Alt2', or 'Alt3'");
            }
        }
    }

    /// <summary>
    /// Validates that final round competitors do not have a callback value
    /// </summary>
    private void ValidateCallbackNotAllowed(List<Competitor> competitors, ValidationContext<Round> context)
    {
        for (var competitorIndex = 0; competitorIndex < competitors.Count; competitorIndex++)
        {
            var competitor = competitors[competitorIndex];
            var propertyPath = $"Competitors[{competitorIndex}].Callback";

            if (competitor.Callback.HasValue)
            {
                context.AddFailure(propertyPath,
                    "Callback must be null for final rounds");
            }
        }
    }
}

