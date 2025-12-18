using System.Text.Json.Serialization;
using Wsdc.Submissions.Models.Enums;

namespace Wsdc.Submissions.Models.Dtos;

/// <summary>
/// Summary information for a division in a submission result
/// </summary>
public class DivisionSummary
{
    /// <summary>
    /// Type of the division
    /// </summary>
    [JsonPropertyName("divisionType")]
    public DivisionType DivisionType { get; set; }

    #region Number of Contestants (from first round)

    /// <summary>
    /// Distinct count of leaders from the first round
    /// </summary>
    [JsonPropertyName("leaderCount")]
    public int LeaderCount { get; set; }

    /// <summary>
    /// Distinct count of followers from the first round
    /// </summary>
    [JsonPropertyName("followerCount")]
    public int FollowerCount { get; set; }

    /// <summary>
    /// Total participants (leaders + followers) from the first round
    /// </summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    #endregion

    #region Round Indicators

    /// <summary>
    /// Indicates whether this division had a prelims round
    /// </summary>
    [JsonPropertyName("hasPrelims")]
    public bool HasPrelims { get; set; }

    /// <summary>
    /// Indicates whether this division had a quarters round
    /// </summary>
    [JsonPropertyName("hasQuarters")]
    public bool HasQuarters { get; set; }

    /// <summary>
    /// Indicates whether this division had a semis round
    /// </summary>
    [JsonPropertyName("hasSemis")]
    public bool HasSemis { get; set; }

    /// <summary>
    /// Number of competitors (couples) in the finals round (0 if no finals)
    /// </summary>
    [JsonPropertyName("finalsCompetitorCount")]
    public int FinalsCompetitorCount { get; set; }

    #endregion
}

