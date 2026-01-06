using Swashbuckle.AspNetCore.Filters;
using Wsdc.Submissions.Models.Dtos;
using Wsdc.Submissions.Models.Enums;

namespace Wsdc.Submissions.Apps.RestAPI.Swagger;

/// <summary>
/// Provides a single example request payload for EventSubmissionRequest in Swagger documentation
/// Based on reference data from event-data.json with anonymized names and IDs
/// </summary>
public class EventSubmissionRequestExample : IExamplesProvider<EventResultsRequest>
{
    #region Score Constants
    private const string ScoreYes = "10";
    private const string ScoreNo = "0";
    private const string ScoreAlt1 = "4.5";
    private const string ScoreAlt2 = "4.3";
    private const string ScoreAlt3 = "4.2";
    #endregion

    /// <summary>
    /// Returns the example EventSubmissionRequest
    /// </summary>
    public EventResultsRequest GetExamples()
    {
        return new EventResultsRequest
        {
            Event = new Event
            {
                EventName = "The Test Event",
                EventStartDate = new DateTime(2025, 5, 2, 0, 0, 0, DateTimeKind.Utc),
                EventEndDate = new DateTime(2025, 5, 4, 0, 0, 0, DateTimeKind.Utc),
                VenueName = "The address of the event venue.",
                VenueAddress1 = "Venue address line 1",
                VenueAddress2 = "Venue address line 2",
                VenueCity = "Venue City",
                VenueCountry = "US",
                VenuePostalCode = "12345",
                VenueStateOrProvince = "IL",
                Staff = CreateStaffList(),
                DivisionsRegistry = new List<Division>
                {
                    new Division
                    {
                        Type = DivisionType.Novice,
                        TypeSecondary = DivisionType.Undefined,
                        Rounds = new List<Round>
                        {
                            CreatePrelimRound(),
                            CreateFinalRound()
                        }
                    }
                }
            }
        };
    }

    #region Staff Creation
    /// <summary>
    /// Creates the staff list with anonymized names
    /// </summary>
    private static List<Staff> CreateStaffList()
    {
        return new List<Staff>
        {
            new Staff { Id = "", Type = StaffType.SCORE_PERSON, Name = "Staff000" },
            new Staff { Id = "", Type = StaffType.CHIEF_JUDGE_PRIMARY, Name = "Staff001" },
            new Staff { Id = "", Type = StaffType.CHIEF_JUDGE_PRIMARY, Name = "Staff002" }
        };
    }
    #endregion

    #region Prelim Round Creation
    /// <summary>
    /// Creates the prelim round with all competitors
    /// </summary>
    private static Round CreatePrelimRound()
    {
        return new Round
        {
            Type = RoundType.Prelims,
            ScoreCaptureType = ScoreCaptureType.Digital,
            Competitors = CreatePrelimCompetitors()
        };
    }

    /// <summary>
    /// Creates all prelim competitors
    /// </summary>
    private static List<Competitor> CreatePrelimCompetitors()
    {
        var competitors = new List<Competitor>();
        competitors.AddRange(CreateLeaderPrelimCompetitors());
        competitors.AddRange(CreateFollowerPrelimCompetitors());
        return competitors;
    }

    /// <summary>
    /// Creates prelim competitors for Leaders (000-019)
    /// Leaders 000-009 make finals, 010-019 do not
    /// </summary>
    private static List<Competitor> CreateLeaderPrelimCompetitors()
    {
        return new List<Competitor>
        {
            // Leaders who make finals (Yes callback)
            CreatePrelimCompetitor("100", ParticipantType.Leader, "First100 Last100", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("001", ParticipantType.Leader, "First001 Last001", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("002", ParticipantType.Leader, "First002 Last002", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("003", ParticipantType.Leader, "First003 Last003", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("004", ParticipantType.Leader, "First004 Last004", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("005", ParticipantType.Leader, "First005 Last005", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("006", ParticipantType.Leader, "First006 Last006", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("007", ParticipantType.Leader, "First007 Last007", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("008", ParticipantType.Leader, "First008 Last008", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("009", ParticipantType.Leader, "First009 Last009", CallbackType.Yes, ScoreYes),
            // Leaders who do not make finals (alternates and no callbacks)
            CreatePrelimCompetitor("010", ParticipantType.Leader, "First010 Last010", CallbackType.Alt1, ScoreAlt1),
            CreatePrelimCompetitor("011", ParticipantType.Leader, "First011 Last011", CallbackType.Alt2, ScoreAlt2),
            CreatePrelimCompetitor("012", ParticipantType.Leader, "First012 Last012", CallbackType.Alt3, ScoreAlt3),
            CreatePrelimCompetitor("013", ParticipantType.Leader, "First013 Last013", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("014", ParticipantType.Leader, "First014 Last014", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("015", ParticipantType.Leader, "First015 Last015", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("016", ParticipantType.Leader, "First016 Last016", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("017", ParticipantType.Leader, "First017 Last017", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("018", ParticipantType.Leader, "First018 Last018", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("019", ParticipantType.Leader, "First019 Last019", CallbackType.No, ScoreNo)
        };
    }

    /// <summary>
    /// Creates prelim competitors for Followers (020-045)
    /// Followers 020-029 make finals (028 has blank ID), 030-045 do not
    /// </summary>
    private static List<Competitor> CreateFollowerPrelimCompetitors()
    {
        return new List<Competitor>
        {
            // Followers who make finals (Yes callback)
            CreatePrelimCompetitor("020", ParticipantType.Follower, "First020 Last020", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("021", ParticipantType.Follower, "First021 Last021", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("022", ParticipantType.Follower, "First022 Last022", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("023", ParticipantType.Follower, "First023 Last023", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("024", ParticipantType.Follower, "First024 Last024", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("025", ParticipantType.Follower, "First025 Last025", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("026", ParticipantType.Follower, "First026 Last026", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("027", ParticipantType.Follower, "First027 Last027", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("", ParticipantType.Follower, "First028 Last028", CallbackType.Yes, ScoreYes),
            CreatePrelimCompetitor("029", ParticipantType.Follower, "First029 Last029", CallbackType.Yes, ScoreYes),
            // Followers who do not make finals (alternates and no callbacks)
            CreatePrelimCompetitor("030", ParticipantType.Follower, "First030 Last030", CallbackType.Alt1, ScoreAlt1),
            CreatePrelimCompetitor("031", ParticipantType.Follower, "First031 Last031", CallbackType.Alt2, ScoreAlt2),
            CreatePrelimCompetitor("032", ParticipantType.Follower, "First032 Last032", CallbackType.Alt3, ScoreAlt3),
            CreatePrelimCompetitor("033", ParticipantType.Follower, "First033 Last033", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("034", ParticipantType.Follower, "First034 Last034", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("035", ParticipantType.Follower, "First035 Last035", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("036", ParticipantType.Follower, "First036 Last036", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("037", ParticipantType.Follower, "First037 Last037", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("038", ParticipantType.Follower, "First038 Last038", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("039", ParticipantType.Follower, "First039 Last039", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("040", ParticipantType.Follower, "First040 Last040", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("041", ParticipantType.Follower, "First041 Last041", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("042", ParticipantType.Follower, "First042 Last042", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("043", ParticipantType.Follower, "First043 Last043", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("044", ParticipantType.Follower, "First044 Last044", CallbackType.No, ScoreNo),
            CreatePrelimCompetitor("045", ParticipantType.Follower, "First045 Last045", CallbackType.No, ScoreNo)
        };
    }

    /// <summary>
    /// Creates a single prelim competitor with standard judges (4 judges for prelims)
    /// </summary>
    private static Competitor CreatePrelimCompetitor(string participantId, ParticipantType type, string name, CallbackType callback, string score)
    {
        return new Competitor
        {
            Callback = callback,
            Participants = new List<Participant>
            {
                new Participant { Id = participantId, Type = type, Name = name }
            },
            Judges = new List<Judge>
            {
                new Judge { Id = "100", Name = "Judge000", Score = score, Violation = "" },
                new Judge { Id = "101", Name = "Judge001", Score = score, Violation = "" },
                new Judge { Id = "102", Name = "Judge002", Score = score, Violation = "" },
                new Judge { Id = "103", Name = "Judge003", Score = score, Violation = "" }
            }
        };
    }
    #endregion

    #region Final Round Creation
    /// <summary>
    /// Creates the final round with all competitors
    /// </summary>
    private static Round CreateFinalRound()
    {
        return new Round
        {
            Type = RoundType.Finals,
            ScoreCaptureType = ScoreCaptureType.Digital,
            Competitors = CreateFinalCompetitors()
        };
    }

    /// <summary>
    /// Creates all final round competitors (same participants as prelim Yes callbacks)
    /// </summary>
    private static List<Competitor> CreateFinalCompetitors()
    {
        return new List<Competitor>
        {
            CreateFinalCompetitor("100", "First100 Last100", "020", "First020 Last020", "3", "1", "1", "7", "2"),
            CreateFinalCompetitor("001", "First001 Last001", "021", "First021 Last021", "6", "4", "3", "1", "3"),
            CreateFinalCompetitor("002", "First002 Last002", "022", "First022 Last022", "4", "6", "2", "3", "8"),
            CreateFinalCompetitor("003", "First003 Last003", "023", "First023 Last023", "7", "5", "4", "10", "4"),
            CreateFinalCompetitor("004", "First004 Last004", "024", "First024 Last024", "5", "3", "8", "5", "9"),
            CreateFinalCompetitor("005", "First005 Last005", "025", "First025 Last025", "1", "10", "5", "8", "6"),
            CreateFinalCompetitor("006", "First006 Last006", "026", "First026 Last026", "8", "8", "6", "2", "5"),
            CreateFinalCompetitor("007", "First007 Last007", "027", "First027 Last027", "2", "2", "10", "9", "7"),
            CreateFinalCompetitor("008", "First008 Last008", "", "First028 Last028", "10", "9", "7", "4", "1"),
            CreateFinalCompetitor("009", "First009 Last009", "029", "First029 Last029", "9", "7", "9", "6", "10")
        };
    }

    /// <summary>
    /// Creates a single final round competitor with leader/follower pair and 5 judges
    /// </summary>
    private static Competitor CreateFinalCompetitor(
        string leaderId, string leaderName,
        string followerId, string followerName,
        string score1, string score2, string score3, string score4, string score5)
    {
        return new Competitor
        {
            Callback = null,
            Participants = new List<Participant>
            {
                new Participant { Id = leaderId, Type = ParticipantType.Leader, Name = leaderName },
                new Participant { Id = followerId, Type = ParticipantType.Follower, Name = followerName }
            },
            Judges = new List<Judge>
            {
                new Judge { Id = "100", Name = "Judge000", Score = score1, Violation = "" },
                new Judge { Id = "101", Name = "Judge001", Score = score2, Violation = "" },
                new Judge { Id = "102", Name = "Judge002", Score = score3, Violation = "" },
                new Judge { Id = "103", Name = "Judge003", Score = score4, Violation = "" },
                new Judge { Id = "104", Name = "Judge004", Score = score5, Violation = "" }
            }
        };
    }
    #endregion
}

