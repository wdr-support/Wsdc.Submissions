using System.Text.Json.Serialization;
using Wsdc.Submissions.Models.Enums;

namespace Wsdc.Submissions.Models.Dtos;

/// <summary>
/// Represents a staff member at an event
/// </summary>
public class Staff
{
    /// <summary>
    /// WSDC ID of the staff member (numeric string for valid IDs, 'NONE' for missing IDs, or empty string)
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Type of staff role
    /// </summary>
    [JsonPropertyName("type")]
    public StaffType Type { get; set; }

    /// <summary>
    /// Full name of the staff member
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email of the staff member
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Phone number of the staff member
    /// </summary>
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;
}

