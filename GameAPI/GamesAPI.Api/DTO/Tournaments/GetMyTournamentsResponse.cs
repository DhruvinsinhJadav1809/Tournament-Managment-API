public class GetMyTournamentsResponse
{
    public List<MyTournamentResponse>
        ActiveTournaments
    {
        get;
        set;
    } = [];

    public List<MyTournamentResponse>
        CompletedTournaments
    {
        get;
        set;
    } = [];
}