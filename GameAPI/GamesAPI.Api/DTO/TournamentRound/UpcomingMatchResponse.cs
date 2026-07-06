public class UpcomingMatchResponse
{
    public Guid MatchId { get; set; }

    public Guid TournamentId { get; set; }

    public string TournamentName { get; set; }
        = string.Empty;

    public string RoundName { get; set; }
        = string.Empty;

    public string? OpponentName { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;
    public string Player1Name { get; set; }
           = string.Empty;
    public string Player2Name { get; set; } = string.Empty;
    public int RoundNumber { get; set; }
    public DateTime? MatchDate { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}