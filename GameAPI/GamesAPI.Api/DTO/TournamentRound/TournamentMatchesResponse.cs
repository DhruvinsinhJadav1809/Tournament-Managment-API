public class TournamentMatchesResponse
{
    public Guid TournamentId { get; set; }

    public string TournamentName { get; set; }
        = string.Empty;

    public List<RoundResponse>
        Rounds
    { get; set; }
            = [];
}