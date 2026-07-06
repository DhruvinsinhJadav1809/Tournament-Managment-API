using GamesAPI.Api.Models;

public class TournamentMatch
{
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public Guid RoundId { get; set; }

    public Guid? Player1Id { get; set; }

    public Guid? Player2Id { get; set; }

    public int? Player1Score { get; set; }

    public int? Player2Score { get; set; }

    public Guid? WinnerId { get; set; }

    public Guid? PreviousMatch1Id { get; set; }

    public Guid? PreviousMatch2Id { get; set; }

    public DateTime? MatchDate { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string Status { get; set; } = "Pending";

    public bool IsBye { get; set; }
    public int MatchNumber { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
        = DateTime.Now;

    public virtual Tournament Tournament { get; set; }
        = null!;

    public virtual TournamentRound Round { get; set; }
        = null!;

    public virtual User? Player1 { get; set; }

    public virtual User? Player2 { get; set; }

    public virtual User? Winner { get; set; }


    public virtual TournamentMatch? PreviousMatch1 { get; set; }

    public virtual TournamentMatch? PreviousMatch2 { get; set; }
}