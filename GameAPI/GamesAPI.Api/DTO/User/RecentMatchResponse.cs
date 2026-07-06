public class RecentMatchResponse
{
    public Guid MatchId { get; set; }

    public string OpponentName { get; set; } = "";

    public string TournamentName { get; set; } = "";

    public string Result { get; set; } = "";

    public string Score { get; set; } = "";

    public DateTime MatchDate { get; set; }
}