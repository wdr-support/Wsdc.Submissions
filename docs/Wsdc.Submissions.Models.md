# Wsdc.Submissions.Models

Model documentation for the WSDC Submissions system.

---

## Domain

### Account

Represents an account that can authenticate via API key

| Property | Type | Description |
|----------|------|-------------|
| Id | `Guid` | Unique identifier for the account |
| Name | `string` | Display name of the account holder |
| Email | `string` | Email address of the account holder |
| HashedApiKey | `string` | BCrypt hashed API key for authentication verification |
| ApiKeyHashLookup | `string` | SHA256 hash of the API key for fast lookups (non-salted, indexable). Used to quickly locate the account before BCrypt verification. |
| IsActive | `bool` | Indicates whether the account is active and can authenticate |

---

## Dtos

### Competitor

Represents a competitor (individual or couple) in a round

| Property | Type | Description |
|----------|------|-------------|
| Participants | `List<Participant>` | List of participants (dancers) in this competitor entry |
| Callback | `Nullable<CallbackType>` | Callback status for this competitor (used in preliminary rounds, must be null for finals). Valid values: 'Yes', 'No', 'Alt1', 'Alt2', 'Alt3' |
| Judges | `List<Judge>` | List of judge scores for this competitor |

### Division

Represents a registry division (e.g., Novice, Intermediate, Advanced, AllStar, etc.)

| Property | Type | Description |
|----------|------|-------------|
| Type | `DivisionType` | Type of the division |
| TypeSecondary | `DivisionType` | Used to indicate the higher level of a combined division. Normally set to Undefined by default. For Example, the combined divison AllStar / Advanced should have the Type field set to Advanced (The lower of the two combined divisions) and the TypeSecondary field would be set to AllStar (The higher of the two combined divisions). |
| Rounds | `List<Round>` | List of rounds in this division |

### DivisionSummary

Summary information for a division in a submission result

| Property | Type | Description |
|----------|------|-------------|
| DivisionType | `DivisionType` | Type of the division |
| LeaderCount | `int` | Distinct count of leaders from the first round |
| FollowerCount | `int` | Distinct count of followers from the first round |
| TotalCount | `int` | Total participants (leaders + followers) from the first round |
| HasPrelims | `bool` | Indicates whether this division had a prelims round |
| HasQuarters | `bool` | Indicates whether this division had a quarters round |
| HasSemis | `bool` | Indicates whether this division had a semis round |
| FinalsCompetitorCount | `int` | Number of competitors (couples) in the finals round (0 if no finals) |

### Event

Represents a dance event with all its details

| Property | Type | Description |
|----------|------|-------------|
| EventName | `string` | Name of the event |
| EventStartDate | `DateTime` | Start date of the event (ISO 8601 format) |
| EventEndDate | `DateTime` | End date of the event (ISO 8601 format) |
| VenueName | `string` | Name of the venue / hotel where the event takes place. |
| VenueAddress1 | `string` | Address line 1 of the venue / hotel where the event takes place. |
| VenueAddress2 | `string` | Address line 2 of the venue / hotel where the event takes place. |
| VenueCity | `string` | City of the venue / hotel where the event takes place. |
| VenueStateOrProvince | `string` | State / Province of the venue / hotel where the event takes place. |
| VenuePostalCode | `string` | Postal code of the venue / hotel where the event takes place. |
| VenueCountry | `string` | ISO 3166 2 letter country code of the venue / hotel where the event takes place. |
| Staff | `List<Staff>` | List of staff members at the event |
| DivisionsRegistry | `List<Division>` | List of registry divisions at the event |
| DivisionsNonRegistry | `List<string>` | List of non registry divisions at the event |

### EventResultsRequest

Root request object for event results submission

| Property | Type | Description |
|----------|------|-------------|
| Event | `Event` | The event results being submitted |

### Judge

Represents a judge's score for a competitor

| Property | Type | Description |
|----------|------|-------------|
| Id | `string` | WSDC ID of the judge (numeric string for valid IDs, 'NONE' for missing IDs, or empty string) |
| Name | `string` | Full name of the judge |
| Score | `string` | Score value - string representing score For non-finals: "10", "4.5", "4.3", "4.2", "0" For finals: positive integer as string (e.g., "1", "2", "3") Validated by FluentValidation based on round type |
| Violation | `string` | Violation note (typically empty) |

### Participant

Represents a participant (dancer) in a competition

| Property | Type | Description |
|----------|------|-------------|
| Id | `string` | WSDC ID of the participant (numeric string for valid IDs, 'NONE' for missing IDs, or empty string) |
| Type | `ParticipantType` | Role of the participant (Leader or Follower) |
| Name | `string` | Full name of the participant |
| HasValidWsdcId | `bool` | Helper property to check if participant has a valid WSDC ID |
| WsdcIdValue | `Nullable<int>` | Helper property to get the WSDC ID as an integer if valid |

### Round

Represents a competition round (Prelim, Quarterfinal, Semifinal, Final)

| Property | Type | Description |
|----------|------|-------------|
| Type | `RoundType` | Type of round (Prelim, Quarterfinal, Semifinal, or Final) |
| ScoreCaptureType | `ScoreCaptureType` | Method used to capture scores (Digital or Paper) |
| Competitors | `List<Competitor>` | List of competitors in this round |

### Staff

Represents a staff member at an event

| Property | Type | Description |
|----------|------|-------------|
| Id | `string` | WSDC ID of the staff member (numeric string for valid IDs, 'NONE' for missing IDs, or empty string) |
| Type | `StaffType` | Type of staff role |
| Name | `string` | Full name of the staff member |
| Email | `string` | Email of the staff member |
| Phone | `string` | Phone number of the staff member |

### SubmissionResultResponse

Response returned after successful submission of event results

| Property | Type | Description |
|----------|------|-------------|
| SubmittedAt | `DateTime` | Timestamp when the results were submitted |
| Divisions | `List<DivisionSummary>` | Summary information for each division in the submission |
| TotalLeaders | `int` | Total number of leaders across all divisions (from first round of each) |
| TotalFollowers | `int` | Total number of followers across all divisions (from first round of each) |
| TotalParticipants | `int` | Total number of participants across all divisions |
| SubmitterName | `string` | Name of the authenticated user who submitted the results |
| SubmitterEmail | `string` | Email of the authenticated user who submitted the results |

---

## Enums

### CallbackType

Callback values for preliminary rounds

| Value | Integer | Description |
|-------|---------|-------------|
| Yes | `1` |  |
| No | `2` |  |
| Alt1 | `3` |  |
| Alt2 | `4` |  |
| Alt3 | `5` |  |

### DivisionType

Type of competition division

| Value | Integer | Description |
|-------|---------|-------------|
| Undefined | `0` |  |
| Newcomer | `1` |  |
| Novice | `2` |  |
| Intermediate | `3` |  |
| Advanced | `4` |  |
| AllStar | `5` |  |
| Champions | `6` |  |
| Masters | `7` |  |
| Juniors | `8` |  |
| Sophisticated | `9` |  |

### ParticipantType

Role of a participant in a competition

| Value | Integer | Description |
|-------|---------|-------------|
| Leader | `1` |  |
| Follower | `2` |  |

### RoundType

Type of competition round

| Value | Integer | Description |
|-------|---------|-------------|
| Prelims | `1` |  |
| Quarters | `2` |  |
| Semis | `3` |  |
| Finals | `4` |  |

### ScoreCaptureType

Method used to capture scores at an event

| Value | Integer | Description |
|-------|---------|-------------|
| Digital | `1` |  |
| Paper | `2` |  |

### StaffType

Type of staff member at an event

| Value | Integer | Description |
|-------|---------|-------------|
| EVENT_DIRECTOR | `1` |  |
| CHIEF_JUDGE_PRIMARY | `2` |  |
| CHIEF_JUDGE_SECONDARY | `3` |  |
| RAW_SCORE_JUDGE | `4` |  |
| SCORE_PERSON | `5` |  |

