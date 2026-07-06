public class RoundResponse
{
    public Guid RoundId { get; set; }

    public string RoundName { get; set; }
        = string.Empty;

    public int RoundNumber { get; set; }

    public List<MatchResponse>
        Matches
    { get; set; }
            = [];
}