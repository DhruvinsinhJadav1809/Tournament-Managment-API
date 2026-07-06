using GamesAPI.Api.Models;

public class TournamentParticipant
{
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public Guid UserId { get; set; }

    public DateTime JoinedAt { get; set; }

    public bool IsWithdrawn { get; set; }

    public DateTime? WithdrawnAt { get; set; }

    public DateTime CreatedAt { get; set; } =
        DateTime.Now;

    public virtual Tournament Tournament { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}