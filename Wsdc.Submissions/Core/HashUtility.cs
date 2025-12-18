using System.Security.Cryptography;
using System.Text;

namespace Wsdc.Submissions.Core;

/// <summary>
/// Utility class for cryptographic hash operations.
/// </summary>
public static class HashUtility
{
    /// <summary>
    /// Computes SHA256 hash of the input string and returns it as a lowercase hex string.
    /// </summary>
    /// <param name="input">The string to hash</param>
    /// <returns>Lowercase hex string representation of the SHA256 hash</returns>
    public static string ComputeSha256Hash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}

