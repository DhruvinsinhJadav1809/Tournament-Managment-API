public class AdminUpcomingMatchResponse
{
    public Guid MatchId { get; set; }

    public Guid TournamentId { get; set; }

    public string TournamentName { get; set; } = "";

    public string Player1Name { get; set; } = "";

    public string Player2Name { get; set; } = "";

    public int RoundNumber { get; set; }

    public DateTime MatchDate { get; set; }

    public DateTime? StartTime { get; set; }
}