public class AdminDashboardResponse
{
    public int TotalTournaments { get; set; }

    public int ActiveTournaments { get; set; }

    public int CompletedTournaments { get; set; }

    public int TotalPlayers { get; set; }

    public int PendingMatches { get; set; }

    public int CompletedMatches { get; set; }
}