public class MatchResponse
{
    public Guid MatchId { get; set; }

    public int MatchNumber { get; set; }

    public Guid? Player1Id { get; set; }

    public string? Player1Name { get; set; }

    public Guid? Player2Id { get; set; }
    public int? Player1Score { get; set; }
    public int? Player2Score { get; set; }

    public string? Player2Name { get; set; }

    public Guid? WinnerId { get; set; }

    public string Status { get; set; }
        = string.Empty;
    public int RoundNumber { get; set; }

    public string RoundName { get; set; }
        = string.Empty;
    public DateTime? MatchDate { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool IsBye { get; set; }
}