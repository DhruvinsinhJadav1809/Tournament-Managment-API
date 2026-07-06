public class PendingActionResponse
{
    public Guid MatchId { get; set; }

    public Guid TournamentId { get; set; }

    public string TournamentName { get; set; } = "";

    public string ActionType { get; set; } = "";

    public string Message { get; set; } = "";

    public DateTime? MatchDate { get; set; }
}