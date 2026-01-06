using System.Text.Json.Serialization;
using Wsdc.Submissions.Models.Enums;

namespace Wsdc.Submissions.Models.Dtos;

/// <summary>
/// Represents a registry division (e.g., Novice, Intermediate, Advanced, AllStar, etc.)
/// </summary>
public class Division
{
    /// <summary>
    /// Type of the division
    /// </summary>
    [JsonPropertyName("type")]
    public DivisionType Type { get; set; }

    /// <summary>
    /// Used to indicate the higher level of a combined division.  Normally set to Undefined by default.
    /// For Example, the combined divison AllStar / Advanced should have the Type field set to Advanced (The lower of the two combined divisions) and the TypeSecondary field would be set to AllStar (The higher of the two combined divisions).
    /// </summary>
    [JsonPropertyName("typeSecondary")]
    public DivisionType TypeSecondary { get; set; }

    /// <summary>
    /// List of rounds in this division
    /// </summary>
    [JsonPropertyName("rounds")]
    public List<Round> Rounds { get; set; } = new();
}

