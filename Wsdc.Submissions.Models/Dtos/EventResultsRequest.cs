using System.Text.Json.Serialization;

namespace Wsdc.Submissions.Models.Dtos;

/// <summary>
/// Root request object for event results submission
/// </summary>
public class EventResultsRequest
{
    /// <summary>
    /// The event results being submitted
    /// </summary>
    [JsonPropertyName("event")]
    public Event Event { get; set; } = new();
}

