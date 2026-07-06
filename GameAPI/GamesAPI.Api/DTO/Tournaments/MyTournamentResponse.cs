public class MyTournamentResponse
{
    public Guid TournamentId
    {
        get;
        set;
    }

    public string TournamentName
    {
        get;
        set;
    } = string.Empty;

    public string GameName
    {
        get;
        set;
    } = string.Empty;

    public string Status
    {
        get;
        set;
    } = string.Empty;

    public DateTime? StartDate
    {
        get;
        set;
    }

    public DateTime? EndDate
    {
        get;
        set;
    }

    public bool IsGeneratedMatches
    {
        get;
        set;
    }

    public bool IsChampion
    {
        get;
        set;
    }
}