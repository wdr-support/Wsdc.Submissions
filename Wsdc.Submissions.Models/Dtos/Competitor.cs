using System.Text.Json.Serialization;
using Wsdc.Submissions.Models.Enums;

namespace Wsdc.Submissions.Models.Dtos;

/// <summary>
/// Represents a competitor (individual or couple) in a round
/// </summary>
public class Competitor
{
    /// <summary>
    /// List of participants (dancers) in this competitor entry
    /// </summary>
    [JsonPropertyName("participants")]
    public List<Participant> Participants { get; set; } = new();

    /// <summary>
    /// Callback status for this competitor (used in preliminary rounds, must be null for finals).
    /// Valid values: 'Yes', 'No', 'Alt1', 'Alt2', 'Alt3'
    /// </summary>
    [JsonPropertyName("callback")]
    public CallbackType? Callback { get; set; }

    /// <summary>
    /// List of judge scores for this competitor
    /// </summary>
    [JsonPropertyName("judges")]
    public List<Judge> Judges { get; set; } = new();
}

