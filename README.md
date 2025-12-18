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

## Quick Start

```bash
dotnet build Wsdc.Submissions.sln
cd Wsdc.Submissions.Apps.RestAPI && dotnet run
```

Open `https://localhost:5001/swagger` and use test key: `sk_test_WsdcSubmissions2024SecureKey`

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
| `Wsdc.Submissions` | Core types (`ServiceResponse<T>`, `ServiceError`) |
| `Wsdc.Submissions.Models` | DTOs & enums for event data |
| `Wsdc.Submissions.Services` | Business logic, validation (FluentValidation) |
| `Wsdc.Submissions.Repositories` | Data access interfaces & implementations |
| `Wsdc.Submissions.Apps.RestAPI` | Web API, middleware, Swagger |

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

