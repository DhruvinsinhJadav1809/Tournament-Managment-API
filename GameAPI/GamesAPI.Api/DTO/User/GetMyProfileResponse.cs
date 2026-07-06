public class GetMyProfileResponse
{
    public string FullName { get; set; } = "";

    public string Email { get; set; } = "";

    public string Role { get; set; } = "";

    public int MatchesPlayed { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public decimal WinRate { get; set; }

    public int Championships { get; set; }

    public int ActiveTournaments { get; set; }

    public int CompletedTournaments { get; set; }
    public Boolean HasProfileImage { get; set; }
    public List<RecentMatchResponse> RecentMatches { get; set; }
        = new();
}