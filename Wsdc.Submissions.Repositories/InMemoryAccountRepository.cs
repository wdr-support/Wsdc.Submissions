using Microsoft.Extensions.Logging;
using Wsdc.Submissions.Core;
using Wsdc.Submissions.Models.Domain;
using Wsdc.Submissions.Repositories.Abstract;

namespace Wsdc.Submissions.Repositories;

/// <summary>
/// In-memory implementation of IAccountRepository for development and testing.
/// Seeds a single test user on initialization and logs the API key to the console.
/// </summary>
public class InMemoryAccountRepository : IAccountRepository
{
    private readonly Dictionary<string, Account> _accountsByApiKeyHash;
    private readonly List<Account> _accounts;
    private readonly ILogger<InMemoryAccountRepository> _logger;

    /////////////////////////////////////////////
    // TEST API KEY CONSTANTS
    /////////////////////////////////////////////

    private const string TestApiKeyEnvVar = "TEST_API_KEY";
    private const string TestUserName = "Test User";
    private const string TestUserEmail = "testuser@wsdc.com";

    public InMemoryAccountRepository(ILogger<InMemoryAccountRepository> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _accountsByApiKeyHash = new Dictionary<string, Account>(StringComparer.OrdinalIgnoreCase);
        _accounts = new List<Account>();

        SeedTestData();
    }

    /////////////////////////////////////////////
    // PUBLIC METHODS
    /////////////////////////////////////////////

    /// <inheritdoc/>
    public Task<Account?> GetByApiKeyHashAsync(string apiKeyHashLookup)
    {
        if (string.IsNullOrWhiteSpace(apiKeyHashLookup))
        {
            _logger.LogDebug("API key hash lookup value is empty");
            return Task.FromResult<Account?>(null);
        }

        if (_accountsByApiKeyHash.TryGetValue(apiKeyHashLookup, out var account))
        {
            if (account.IsActive)
            {
                _logger.LogDebug("Found active account for API key hash lookup");
                return Task.FromResult<Account?>(account);
            }

            _logger.LogDebug("Account found but is not active");
        }
        else
        {
            _logger.LogDebug("No account found for API key hash lookup");
        }

        return Task.FromResult<Account?>(null);
    }

    /////////////////////////////////////////////
    // PRIVATE METHODS
    /////////////////////////////////////////////

    private void SeedTestData()
    {
        var testApiKey = Environment.GetEnvironmentVariable(TestApiKeyEnvVar);

        if (string.IsNullOrWhiteSpace(testApiKey))
        {
            _logger.LogWarning("Environment variable {EnvVar} is not set. Test account will not be seeded.", TestApiKeyEnvVar);
            return;
        }

        var hashedApiKey = BCrypt.Net.BCrypt.HashPassword(testApiKey);
        var apiKeyHashLookup = HashUtility.ComputeSha256Hash(testApiKey);

        var testAccount = new Account
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            Name = TestUserName,
            Email = TestUserEmail,
            HashedApiKey = hashedApiKey,
            ApiKeyHashLookup = apiKeyHashLookup,
            IsActive = true
        };

        _accounts.Add(testAccount);
        _accountsByApiKeyHash[apiKeyHashLookup] = testAccount;

        _logger.LogInformation("=======================================================");
        _logger.LogInformation("TEST ACCOUNT SEEDED FOR DEVELOPMENT");
        _logger.LogInformation("User: {UserName} ({Email})", TestUserName, TestUserEmail);
        _logger.LogInformation("=======================================================");
    }
}

