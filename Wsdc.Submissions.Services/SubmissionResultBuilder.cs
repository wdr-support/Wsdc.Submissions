using System.Security.Claims;
using System.Threading;
using Wsdc.Submissions.Extensions;
using Wsdc.Submissions.Models.Dtos;
using Wsdc.Submissions.Models.Enums;
using Wsdc.Submissions.Services.Abstract;

namespace Wsdc.Submissions.Services;

/// <summary>
/// Service for building submission result responses from event data
/// </summary>
public class SubmissionResultBuilder : ISubmissionResultBuilder
{
    /// <inheritdoc/>
    public SubmissionResultResponse Build(EventResultsRequest request)
    {
        var divisionSummaries = BuildDivisionSummaries(request.Event.Divisions);
        var principal = Thread.CurrentPrincipal as ClaimsPrincipal;

        var response = new SubmissionResultResponse
        {
            SubmittedAt = DateTime.UtcNow,
            Divisions = divisionSummaries,
            TotalLeaders = divisionSummaries.Sum(d => d.LeaderCount),
            TotalFollowers = divisionSummaries.Sum(d => d.FollowerCount),
            TotalParticipants = divisionSummaries.Sum(d => d.TotalCount),
            SubmitterName = principal.GetClaimValue(ClaimTypes.Name),
            SubmitterEmail = principal.GetClaimValue(ClaimTypes.Email)
        };

        return response;
    }

    #region Private Helper Methods

    /// <summary>
    /// Builds summary information for all divisions
    /// </summary>
    private List<DivisionSummary> BuildDivisionSummaries(List<Division> divisions)
    {
        return divisions.Select(BuildDivisionSummary).ToList();
    }

    /// <summary>
    /// Builds summary information for a single division
    /// </summary>
    private DivisionSummary BuildDivisionSummary(Division division)
    {
        var firstRound = division.Rounds.FirstOrDefault();
        var (leaderCount, followerCount) = CountParticipantsByRole(firstRound);

        return new DivisionSummary
        {
            DivisionType = division.Type,
            LeaderCount = leaderCount,
            FollowerCount = followerCount,
            TotalCount = leaderCount + followerCount,
            HasPrelims = HasRoundType(division.Rounds, RoundType.Prelims),
            HasQuarters = HasRoundType(division.Rounds, RoundType.Quarters),
            HasSemis = HasRoundType(division.Rounds, RoundType.Semis),
            FinalsCompetitorCount = GetFinalsCompetitorCount(division.Rounds)
        };
    }

    /// <summary>
    /// Checks if the division has a specific round type
    /// </summary>
    private bool HasRoundType(List<Round> rounds, RoundType roundType)
    {
        return rounds.Any(r => r.Type == roundType);
    }

    /// <summary>
    /// Gets the number of competitors in the finals round
    /// </summary>
    private int GetFinalsCompetitorCount(List<Round> rounds)
    {
        var finalsRound = rounds.FirstOrDefault(r => r.Type == RoundType.Finals);
        return finalsRound?.Competitors.Count ?? 0;
    }

    /// <summary>
    /// Counts distinct participants by role from the first round
    /// </summary>
    private (int LeaderCount, int FollowerCount) CountParticipantsByRole(Round? firstRound)
    {
        if (firstRound == null)
        {
            return (0, 0);
        }

        var allParticipants = firstRound.Competitors
            .SelectMany(c => c.Participants)
            .ToList();

        var leaderCount = CountDistinctByRole(allParticipants, ParticipantType.Leader);
        var followerCount = CountDistinctByRole(allParticipants, ParticipantType.Follower);

        return (leaderCount, followerCount);
    }

    /// <summary>
    /// Counts distinct participants for a specific role based on WSDC ID or name
    /// </summary>
    private int CountDistinctByRole(List<Participant> participants, ParticipantType role)
    {
        return participants
            .Where(p => p.Type == role)
            .GroupBy(p => p.HasValidWsdcId ? p.Id : p.Name)
            .Count();
    }

    #endregion
}

