using Wsdc.Submissions.Models.Domain;

namespace Wsdc.Submissions.Repositories.Abstract;

/// <summary>
/// Repository interface for account operations.
/// This is a key integration point into the WSDC system - implement this interface
/// to connect to your actual account/user data store.
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    /// Retrieves an account by its API key hash lookup value (SHA256).
    /// The caller should compute SHA256 of the API key and pass it here for fast lookup.
    /// After retrieval, the caller should verify the API key against HashedApiKey using BCrypt.
    /// </summary>
    /// <param name="apiKeyHashLookup">SHA256 hash of the API key (hex string, lowercase)</param>
    /// <returns>The active account if found, null otherwise</returns>
    Task<Account?> GetByApiKeyHashAsync(string apiKeyHashLookup);
}

