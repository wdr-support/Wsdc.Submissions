namespace Wsdc.Submissions.Models.Domain;

/// <summary>
/// Represents an account that can authenticate via API key
/// </summary>
public class Account
{
    /// <summary>
    /// Unique identifier for the account
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Display name of the account holder
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Email address of the account holder
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// BCrypt hashed API key for authentication verification
    /// </summary>
    public required string HashedApiKey { get; set; }

    /// <summary>
    /// SHA256 hash of the API key for fast lookups (non-salted, indexable).
    /// Used to quickly locate the account before BCrypt verification.
    /// </summary>
    public required string ApiKeyHashLookup { get; set; }

    /// <summary>
    /// Indicates whether the account is active and can authenticate
    /// </summary>
    public bool IsActive { get; set; } = true;
}

