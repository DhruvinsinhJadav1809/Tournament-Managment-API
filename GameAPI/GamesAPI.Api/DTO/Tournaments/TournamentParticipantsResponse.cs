public class TournamentParticipantsResponse
{
    public Guid TournamentId { get; set; }

    public string TournamentName { get; set; } = string.Empty;

    public int MaxParticipants { get; set; }

    public int CurrentParticipants { get; set; }

    public List<TournamentParticipantResponse>
        Participants
    { get; set; } = [];
}