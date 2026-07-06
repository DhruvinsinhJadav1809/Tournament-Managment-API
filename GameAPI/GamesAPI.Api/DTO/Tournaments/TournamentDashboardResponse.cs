public class TournamentDashboardResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string GameName { get; set; } = string.Empty;

    public string TournamentType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int MaxParticipants { get; set; }

    public int CurrentParticipants { get; set; }

    public DateTime? RegistrationStartDate { get; set; }

    public DateTime? RegistrationEndDate { get; set; }
    public bool IsParticipated { get; set; }

    public DateTime? StartDate { get; set; }
    public bool IsGeneratedMatches
    {
        get;
        set;
    }
}