using System.Text.Json.Serialization;
using Wsdc.Submissions.Models.Enums;

namespace Wsdc.Submissions.Models.Dtos;

/// <summary>
/// Represents a dance event with all its details
/// </summary>
public class Event
{
    /// <summary>
    /// Name of the event
    /// </summary>
    [JsonPropertyName("eventName")]
    public string EventName { get; set; } = string.Empty;

    /// <summary>
    /// Start date of the event (ISO 8601 format)
    /// </summary>
    [JsonPropertyName("eventStartDate")]
    public DateTime EventStartDate { get; set; }

    /// <summary>
    /// End date of the event (ISO 8601 format)
    /// </summary>
    [JsonPropertyName("eventEndDate")]
    public DateTime EventEndDate { get; set; }

    /// <summary>
    /// Venue and address where the event takes place
    /// </summary>
    [JsonPropertyName("eventLocation")]
    public string EventLocation { get; set; } = string.Empty;

    /// <summary>
    /// Method used to capture scores (Digital or Paper)
    /// </summary>
    [JsonPropertyName("scoreCaptureType")]
    public ScoreCaptureType ScoreCaptureType { get; set; }

    /// <summary>
    /// List of staff members at the event
    /// </summary>
    [JsonPropertyName("staff")]
    public List<Staff> Staff { get; set; } = new();

    /// <summary>
    /// List of competition divisions at the event
    /// </summary>
    [JsonPropertyName("divisions")]
    public List<Division> Divisions { get; set; } = new();
}

