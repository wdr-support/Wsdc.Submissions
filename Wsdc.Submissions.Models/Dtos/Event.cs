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
    /// Name of the venue / hotel where the event takes place.
    /// </summary>
    [JsonPropertyName("venueName")]
    public string VenueName { get; set; } = string.Empty;

    /// <summary>
    /// Address line 1 of the venue / hotel where the event takes place.
    /// </summary>
    [JsonPropertyName("venueAddress1")]
    public string VenueAddress1 { get; set; } = string.Empty;

    /// <summary>
    /// Address line 2 of the venue / hotel where the event takes place.
    /// </summary>
    [JsonPropertyName("venueAddress2")]
    public string VenueAddress2 { get; set; } = string.Empty;

    /// <summary>
    /// City of the venue / hotel where the event takes place.
    /// </summary>
    [JsonPropertyName("venueCity")]
    public string VenueCity { get; set; } = string.Empty;

    /// <summary>
    /// State / Province of the venue / hotel where the event takes place.
    /// </summary>
    [JsonPropertyName("venueStateOrProvince")]
    public string VenueStateOrProvince { get; set; } = string.Empty;

    /// <summary>
    /// Postal code of the venue / hotel where the event takes place.
    /// </summary>
    [JsonPropertyName("venuePostalCode")]
    public string VenuePostalCode { get; set; } = string.Empty;

    /// <summary>
    /// ISO 3166 2 letter country code of the venue / hotel where the event takes place.
    /// </summary>
    [JsonPropertyName("venueCountry")]
    public string VenueCountry { get; set; } = string.Empty;

    /// <summary>
    /// List of staff members at the event
    /// </summary>
    [JsonPropertyName("staff")]
    public List<Staff> Staff { get; set; } = new();

    /// <summary>
    /// List of registry divisions at the event
    /// </summary>
    [JsonPropertyName("divisionsRegistry")]
    public List<Division> DivisionsRegistry { get; set; } = new();

    /// <summary>
    /// List of non registry divisions at the event
    /// </summary>
    [JsonPropertyName("divisionsNonRegistry")]
    public List<string> DivisionsNonRegistry { get; set; } = new();
}

