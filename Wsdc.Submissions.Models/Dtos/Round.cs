using System.Text.Json.Serialization;
using Wsdc.Submissions.Models.Enums;

namespace Wsdc.Submissions.Models.Dtos;

/// <summary>
/// Represents a competition round (Prelim, Quarterfinal, Semifinal, Final)
/// </summary>
public class Round
{
    /// <summary>
    /// Type of round (Prelim, Quarterfinal, Semifinal, or Final)
    /// </summary>
    [JsonPropertyName("type")]
    public RoundType Type { get; set; }

    /// <summary>
    /// List of competitors in this round
    /// </summary>
    [JsonPropertyName("competitors")]
    public List<Competitor> Competitors { get; set; } = new();
}

