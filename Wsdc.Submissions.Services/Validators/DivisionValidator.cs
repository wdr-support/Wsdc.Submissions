using FluentValidation;
using Wsdc.Submissions.Models.Dtos;

namespace Wsdc.Submissions.Services.Validators;

/// <summary>
/// Validator for Division DTO
/// </summary>
public class DivisionValidator : AbstractValidator<Division>
{
    public DivisionValidator()
    {
        // DIVISION TYPE VALIDATION
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Division type must be one of: Newcomer, Novice, Intermediate, Advanced, AllStar, Champions, Masters, Juniors, Sophisticated")
            .WithErrorCode("INVALID_DIVISION_TYPE");

        // ROUNDS VALIDATION
        RuleFor(x => x.Rounds)
            .NotNull()
            .WithMessage("Rounds list is required")
            .WithErrorCode("ROUNDS_REQUIRED");

        RuleFor(x => x.Rounds)
            .NotEmpty()
            .WithMessage("Division must have at least one round")
            .WithErrorCode("NO_ROUNDS");

        RuleForEach(x => x.Rounds)
            .SetValidator(new RoundValidator());
    }
}

