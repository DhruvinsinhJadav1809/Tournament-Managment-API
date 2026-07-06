public class GenerateMatchesRequest
{
    public DateTime StartDate { get; set; }

    public TimeSpan FirstMatchTime { get; set; }

    public int MatchDurationMinutes { get; set; }

    public int MatchesPerDay { get; set; }

    public int BreakMinutes { get; set; }
}