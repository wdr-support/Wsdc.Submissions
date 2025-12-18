using System.Security.Claims;

namespace Wsdc.Submissions.Extensions;

/// <summary>
/// Extension methods for working with ClaimsPrincipal
/// </summary>
public static class ClaimExtensions
{
    /// <summary>
    /// Safely extracts a claim value from the principal with null checks
    /// </summary>
    /// <param name="principal">The claims principal to extract from</param>
    /// <param name="claimType">The type of claim to retrieve</param>
    /// <returns>The claim value if found and authenticated, null otherwise</returns>
    public static string? GetClaimValue(this ClaimsPrincipal? principal, string claimType)
    {
        if (principal?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        return principal.FindFirst(claimType)?.Value;
    }
}

