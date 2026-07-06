public interface IAdminDashboardService
{
    Task<AdminDashboardResponse>
        GetDashboardAsync();

    Task<List<TournamentOverviewResponse>>
        GetTournamentOverviewAsync();

    Task<List<UpcomingMatchResponse>>
        GetUpcomingMatchesAsync();

    Task<List<PendingActionResponse>>
        GetPendingActionsAsync();

    Task<List<TopPlayerResponse>>
        GetTopPlayersAsync();

    Task<List<RecentActivityResponse>>
        GetRecentActivitiesAsync();

    Task<byte[]> ExportTournamentReportAsync(Guid id);
}