using GamesAPI.Api.Models;
public class TournamentRound
{
    public Guid Id { get; set; }

    public Guid TournamentId { get; set; }

    public int RoundNumber { get; set; }

    public string RoundName { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; }
        = DateTime.Now;

    public virtual Tournament Tournament { get; set; }
        = null!;

    public virtual ICollection<TournamentMatch>
        TournamentMatches
    { get; set; }
            = new List<TournamentMatch>();
}