namespace Wsdc.Submissions.Services.Validators.Constants;

/// <summary>
/// Constants for validation operations
/// </summary>
public static class ValidationConstants
{
    #region Error Codes

    /// <summary>
    /// Error code for general validation error
    /// </summary>
    public const string ErrorCodeValidationError = "VALIDATION_ERROR";

    /// <summary>
    /// Error code for internal service error
    /// </summary>
    public const string ErrorCodeInternalError = "INTERNAL_ERROR";

    #endregion

    #region String Length Constraints

    /// <summary>
    /// Maximum length for name fields (EventName, Staff.Name, Participant.Name)
    /// </summary>
    public const int MaxNameLength = 255;

    /// <summary>
    /// Maximum length for location fields (EventLocation)
    /// </summary>
    public const int MaxLocationLength = 512;

    /// <summary>
    /// Maximum length for ID fields (Staff.Id, Participant.Id, Judge.Id)
    /// </summary>
    public const int MaxIdLength = 20;

    /// <summary>
    /// Maximum length for score fields (Judge.Score)
    /// </summary>
    public const int MaxScoreLength = 10;

    /// <summary>
    /// Maximum length for email fields (RFC 5321 standard)
    /// </summary>
    public const int MaxEmailLength = 254;

    /// <summary>
    /// Maximum length for phone fields (international with formatting)
    /// </summary>
    public const int MaxPhoneLength = 30;

    #endregion
}

