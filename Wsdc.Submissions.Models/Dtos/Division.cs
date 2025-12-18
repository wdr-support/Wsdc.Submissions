using System.Text.Json.Serialization;
using Wsdc.Submissions.Models.Enums;

namespace Wsdc.Submissions.Models.Dtos;

/// <summary>
/// Represents a competition division (e.g., Novice, Intermediate, Advanced, AllStar, etc.)
/// </summary>
public class Division
{
    /// <summary>
    /// Type of the division
    /// </summary>
    [JsonPropertyName("type")]
    public DivisionType Type { get; set; }

    /// <summary>
    /// List of rounds in this division
    /// </summary>
    [JsonPropertyName("rounds")]
    public List<Round> Rounds { get; set; } = new();
}

