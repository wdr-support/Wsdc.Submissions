using System.Text.Json.Serialization;

namespace Wsdc.Submissions.Models.Dtos;

/// <summary>
/// Represents a judge's score for a competitor
/// </summary>
public class Judge
{
    /// <summary>
    /// WSDC ID of the judge (numeric string for valid IDs, 'NONE' for missing IDs, or empty string)
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Full name of the judge
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Score value - string representing score
    /// For non-finals: "10", "4.5", "4.3", "4.2", "0"
    /// For finals: positive integer as string (e.g., "1", "2", "3")
    /// Validated by FluentValidation based on round type
    /// </summary>
    [JsonPropertyName("score")]
    public string Score { get; set; } = string.Empty;

    /// <summary>
    /// Violation note (typically empty)
    /// </summary>
    [JsonPropertyName("violation")]
    public string? Violation { get; set; }
}

