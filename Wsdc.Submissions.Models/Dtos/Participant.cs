using System.Text.Json.Serialization;
using Wsdc.Submissions.Models.Enums;

namespace Wsdc.Submissions.Models.Dtos;

/// <summary>
/// Represents a participant (dancer) in a competition
/// </summary>
public class Participant
{
    /// <summary>
    /// WSDC ID of the participant (numeric string for valid IDs, 'NONE' for missing IDs, or empty string)
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Role of the participant (Leader or Follower)
    /// </summary>
    [JsonPropertyName("type")]
    public ParticipantType Type { get; set; }

    /// <summary>
    /// Full name of the participant
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Helper property to check if participant has a valid WSDC ID
    /// </summary>
    [JsonIgnore]
    public bool HasValidWsdcId => int.TryParse(Id, out var id) && id > 0;

    /// <summary>
    /// Helper property to get the WSDC ID as an integer if valid
    /// </summary>
    [JsonIgnore]
    public int? WsdcIdValue => int.TryParse(Id, out var id) && id > 0 ? id : null;
}

