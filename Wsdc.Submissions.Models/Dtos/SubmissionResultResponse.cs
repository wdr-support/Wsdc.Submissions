using System.Text.Json.Serialization;

namespace Wsdc.Submissions.Models.Dtos;

/// <summary>
/// Response returned after successful submission of event results
/// </summary>
public class SubmissionResultResponse
{
    /// <summary>
    /// Timestamp when the results were submitted
    /// </summary>
    [JsonPropertyName("submittedAt")]
    public DateTime SubmittedAt { get; set; }

    /// <summary>
    /// Summary information for each division in the submission
    /// </summary>
    [JsonPropertyName("divisions")]
    public List<DivisionSummary> Divisions { get; set; } = new();

    /// <summary>
    /// Total number of leaders across all divisions (from first round of each)
    /// </summary>
    [JsonPropertyName("totalLeaders")]
    public int TotalLeaders { get; set; }

    /// <summary>
    /// Total number of followers across all divisions (from first round of each)
    /// </summary>
    [JsonPropertyName("totalFollowers")]
    public int TotalFollowers { get; set; }

    /// <summary>
    /// Total number of participants across all divisions
    /// </summary>
    [JsonPropertyName("totalParticipants")]
    public int TotalParticipants { get; set; }

    /// <summary>
    /// Name of the authenticated user who submitted the results
    /// </summary>
    [JsonPropertyName("submitterName")]
    public string? SubmitterName { get; set; }

    /// <summary>
    /// Email of the authenticated user who submitted the results
    /// </summary>
    [JsonPropertyName("submitterEmail")]
    public string? SubmitterEmail { get; set; }
}

