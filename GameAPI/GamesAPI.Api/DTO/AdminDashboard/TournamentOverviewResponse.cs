public class TournamentOverviewResponse
{
    public Guid TournamentId { get; set; }

    public string TournamentName { get; set; } = "";

    public string GameName { get; set; } = "";

    public int Participants { get; set; }

    public string Status { get; set; } = "";

    public int TotalMatches { get; set; }

    public int CompletedMatches { get; set; }

    public int ProgressPercentage { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    public string MatchProgress { get; set; } = "";
}