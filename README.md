# WSDC Submissions API

.NET 8.0 REST API for validating and submitting WSDC dance competition results.

## What This API Does

Events submit competition results (placements, callbacks, scores) to the WSDC for official point tracking. 

This API:

1. **Validates** submitted event data against WSDC business rules
2. **Persists** validated results to the WSDC points registry

---

## Model Hierarchy

```
EventResultsRequest
└── Event
    ├── eventName, eventStartDate, eventEndDate, eventLocation
    ├── scoreCaptureType (Digital | Paper)
    ├── Staff[]
    │   ├── id, name, email, phone
    │   └── type (EVENT_DIRECTOR, CHIEF_JUDGE_PRIMARY, CHIEF_JUDGE_SECONDARY, RAW_SCORE_JUDGE, SCORE_PERSON)
    └── Divisions[]
        ├── type (Newcomer, Novice, Intermediate, Advanced, AllStar, Champions, Masters, Juniors, Sophisticated)
        └── Rounds[]
            ├── type (Prelims, Quarters, Semis, Finals)
            └── Competitors[]
                ├── callback (Yes, No, Alt1, Alt2, Alt3) — prelims only
                ├── Participants[]
                │   ├── id, name
                │   └── type (Leader, Follower)
                └── Judges[]
                    ├── id, name
                    └── score (prelims: "10", "4.5", "4.3", "4.2", "0" | finals: placement "1", "2", etc.)
```

---

## Contributing

### Prerequisites

Before you can contribute to this project, you'll need:

#### 1. Development Environment

**Choose an IDE** (pick one based on your preference):
- [Visual Studio Code](https://code.visualstudio.com/) — Lightweight, cross-platform
- [Visual Studio Community](https://visualstudio.microsoft.com/vs/community/) — Full-featured IDE

#### 2. .NET SDK 8.0

<details>
<summary><b>macOS</b></summary>

Download and install the .NET 8 SDK:
- [.NET 8.0 Downloads](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Direct download: .NET SDK 8.0.417 for macOS (x64)](https://builds.dotnet.microsoft.com/dotnet/Sdk/8.0.417/dotnet-sdk-8.0.417-osx-x64.pkg)

Verify installation:
```bash
dotnet --version  # Should show 8.0.x
```
</details>

<details>
<summary><b>Windows</b></summary>

Download and install the .NET 8 SDK:
- [.NET 8.0 Downloads](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

Verify installation:
```cmd
dotnet --version
```
</details>

#### 3. GitHub Access Token

To commit changes to this repository, you'll need a fine-grained personal access token:

1. Go to [GitHub Settings → Developer settings → Personal access tokens → Fine-grained tokens](https://github.com/settings/tokens?type=beta)
2. Click **"Generate new token"**
3. Configure the token:
   - **Resource owner:** `wsdc-submissions`
   - **Repository access:** Select this repository
   - **Permissions:** Set **Contents** to **Read and write**
   - **Expiration:** Choose an appropriate expiration date
4. Click **"Generate token"** and save it securely

Use this token as your password when pushing commits via HTTPS, or configure it with Git credential manager.

---

## Quick Start

### 1. Set the Test API Key

Before running the application, set the `TEST_API_KEY` environment variable:

<details>
<summary><b>macOS / Linux</b></summary>

```bash
export TEST_API_KEY="sk_test_000000000000000000000000000000"
```
</details>

<details>
<summary><b>Windows (PowerShell)</b></summary>

```powershell
$env:TEST_API_KEY="sk_test_000000000000000000000000000000"
```
</details>

<details>
<summary><b>Windows (Command Prompt)</b></summary>

```cmd
set TEST_API_KEY=sk_test_000000000000000000000000000000
```
</details>

### 2. Build and Run

```bash
dotnet build Wsdc.Submissions.sln
cd Wsdc.Submissions.Apps.RestAPI && dotnet run
```

### 3. Test the API

Open `https://localhost:7017/swagger` and use the test key: `sk_test_000000000000000000000000000000`

In Swagger UI: Click 🔓 **Authorize** → Enter the key → Click **Authorize**

---

## 🔌 Key Integration Points

### 1. `IWsdcSubmissionsService` — Submission Logic
This is where you receive the **validated event model** and persist it to the WSDC system:

```csharp
public interface IWsdcSubmissionsService
{
    Task<ServiceResponse<object>> SubmitEventDataAsync(EventResultsRequest request);
}
```

**Current implementation:** `WsdcSubmissionsService` — Implement your persistence logic here to save validated data to WSDC.

---

### 2. `IAccountRepository` — Authentication Backend
Implement this interface to connect to your account/user data store:

```csharp
public interface IAccountRepository
{
    Task<Account?> GetByApiKeyHashAsync(string apiKeyHashLookup);
}
```

**Account model:** `Id`, `Name`, `Email`, `HashedApiKey` (BCrypt), `ApiKeyHashLookup` (SHA256), `IsActive`

**Current implementation:** `InMemoryAccountRepository` (dev/testing only)

**To integrate:** Create your implementation and register in `Program.cs`:
```csharp
builder.Services.AddScoped<IAccountRepository, YourDatabaseAccountRepository>();
```

---

## Solution Structure

| Project | Purpose |
|---------|---------|
| `Wsdc.Submissions` | Core types (`ServiceResponse<T>`, `ServiceError`), utilities, converters, extensions |
| `Wsdc.Submissions.Models` | DTOs, domain models, and enums for event data |
| `Wsdc.Submissions.Services` | Business logic, validation (FluentValidation), submission services |
| `Wsdc.Submissions.Repositories` | Data access interfaces & implementations (account management) |
| `Wsdc.Submissions.Apps.RestAPI` | Web API, middleware, Swagger, controllers |
| `Wsdc.Submissions.Apps.DocBuilder` | Documentation generator tool for POCO models |

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/results/validate` | Validate event data (no submission) |
| `POST` | `/api/results/submit` | Validate and submit event data |
| `GET` | `/api/schemas/results` | Get JSON Schema for request format |

All endpoints require `Authorization` header (except `/swagger`).

**Response format:** `ServiceResponse<T>` with `data`, `hasErrors`, and `errors[]`

---

## Authentication

Include header: `Authorization: Bearer <your-api-key>`

In Swagger UI: Click 🔓 Authorize → enter key → Authorize

---

## Tech Stack

.NET 8.0 | FluentValidation | NJsonSchema | Serilog | Swashbuckle (OpenAPI)

---

## Roadmap

- [ ] Results persistence (update `IWsdcSubmissionsService`)
- [ ] Account lookup (replace `InMemoryAccountRepository`)

