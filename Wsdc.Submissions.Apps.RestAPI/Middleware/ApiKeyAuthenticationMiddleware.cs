using System.Security.Claims;
using System.Threading;
using Wsdc.Submissions.Core;
using Wsdc.Submissions.Models.Domain;
using Wsdc.Submissions.Repositories.Abstract;

namespace Wsdc.Submissions.Apps.RestAPI.Middleware;

/// <summary>
/// Middleware for authenticating requests using Bearer token in the Authorization header.
/// Uses SHA256 hash for fast account lookup, then verifies with BCrypt.
/// </summary>
public class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

    /////////////////////////////////////////////
    // CONSTANTS
    /////////////////////////////////////////////

    private const string AuthorizationHeader = "Authorization";
    private const string BearerScheme = "Bearer";
    private const string AuthenticationScheme = "Bearer";

    private static readonly string[] PublicPaths = new[]
    {
        "/health",
        "/swagger"
    };

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /////////////////////////////////////////////
    // MIDDLEWARE INVOCATION
    /////////////////////////////////////////////

    public async Task InvokeAsync(HttpContext context, IAccountRepository accountRepository)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Allow public paths without authentication
        if (IsPublicPath(path))
        {
            await _next(context);
            return;
        }

        var apiKey = ExtractBearerToken(context.Request.Headers[AuthorizationHeader].FirstOrDefault());

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("Authorization header missing or invalid in request to {Path}", path);
            await RespondUnauthorizedAsync(context, "Authorization header missing or invalid. Use 'Authorization: Bearer <api-key>'.");
            return;
        }

        var account = await ValidateApiKeyAsync(accountRepository, apiKey);

        if (account == null)
        {
            _logger.LogWarning("Invalid or revoked API Key used for request to {Path}", path);
            await RespondUnauthorizedAsync(context, "Invalid or revoked API Key");
            return;
        }

        // Build claims for the authenticated account
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new Claim(ClaimTypes.Name, account.Name),
            new Claim(ClaimTypes.Email, account.Email)
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        context.User = principal;
        Thread.CurrentPrincipal = principal;
        context.Items["Account"] = account;

        _logger.LogDebug("Authenticated request from {AccountName} ({Email})", account.Name, account.Email);

        await _next(context);
    }

    /////////////////////////////////////////////
    // PRIVATE METHODS
    /////////////////////////////////////////////

    private static bool IsPublicPath(string path)
    {
        return PublicPaths.Any(publicPath =>
            path.StartsWith(publicPath, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Extracts the token from a Bearer authorization header value.
    /// </summary>
    private static string? ExtractBearerToken(string? authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return null;
        }

        if (authorizationHeader.StartsWith(BearerScheme + " ", StringComparison.OrdinalIgnoreCase))
        {
            return authorizationHeader.Substring(BearerScheme.Length + 1).Trim();
        }

        return null;
    }

    /// <summary>
    /// Validates the API key by looking up the account using SHA256 hash,
    /// then verifying with BCrypt.
    /// </summary>
    private async Task<Account?> ValidateApiKeyAsync(
        IAccountRepository accountRepository,
        string apiKey)
    {
        // Compute SHA256 hash for fast lookup
        var apiKeyHashLookup = HashUtility.ComputeSha256Hash(apiKey);

        // Look up account by hash
        var account = await accountRepository.GetByApiKeyHashAsync(apiKeyHashLookup);

        if (account == null)
        {
            return null;
        }

        // Verify with BCrypt for security
        if (BCrypt.Net.BCrypt.Verify(apiKey, account.HashedApiKey))
        {
            return account;
        }

        _logger.LogWarning("API key hash matched but BCrypt verification failed - possible hash collision or data corruption");
        return null;
    }

    private static async Task RespondUnauthorizedAsync(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = "Unauthorized",
            message = message
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}

/////////////////////////////////////////////
// EXTENSION METHOD
/////////////////////////////////////////////

/// <summary>
/// Extension methods for registering API key authentication middleware
/// </summary>
public static class ApiKeyAuthenticationMiddlewareExtensions
{
    /// <summary>
    /// Adds API key authentication middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseApiKeyAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiKeyAuthenticationMiddleware>();
    }
}

