
using Microsoft.EntityFrameworkCore;
using GamesAPI.Api.Data;
using GamesAPI.Api.Exceptions;
using GamesAPI.Api.Constants;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
namespace GamesAPI.Api.Services
{
    [Authorize(Roles = RoleConstants.Admin)]
    public class AdminDashboardService
        : IAdminDashboardService
    {
        private readonly AppDbContext
     _context;

        private readonly IConfiguration
            _configuration;
        private readonly ILogService
                _logService;

        public AdminDashboardService(
            AppDbContext context,
            IConfiguration configuration,
             ILogService logService)
        {
            _context = context;
            _configuration =
                configuration;
            _logService =
          logService;
        }
        public async Task<AdminDashboardResponse>
    GetDashboardAsync()
        {
            var currentDate =
                DateTime.Now;

            var totalTournaments =
                await _context.Tournaments
                    .CountAsync(x =>
                        !x.IsDeleted);

            var activeTournaments =
                await _context.Tournaments
                    .CountAsync(x =>
                        !x.IsDeleted &&
                        currentDate <= x.EndDate);

            var completedTournaments =
                await _context.Tournaments
                    .CountAsync(x =>
                        !x.IsDeleted &&
                        currentDate > x.EndDate);

            var totalPlayers =
                await _context.Users
                    .CountAsync(x =>
                        !x.IsDeleted &&
                        x.IsActive &&
                        x.Role.Name ==
                        RoleConstants.User);

            var pendingMatches =
                await _context.TournamentMatches
                    .CountAsync(x =>
                        x.Player1Id != null &&
                        x.Player2Id != null &&
                        x.WinnerId == null);

            var completedMatches =
                await _context.TournamentMatches
                    .CountAsync(x =>
                        x.WinnerId != null);

            return new AdminDashboardResponse
            {
                TotalTournaments =
                    totalTournaments,

                ActiveTournaments =
                    activeTournaments,

                CompletedTournaments =
                    completedTournaments,

                TotalPlayers =
                    totalPlayers,

                PendingMatches =
                    pendingMatches,

                CompletedMatches =
                    completedMatches
            };
        }

        public async Task<List<PendingActionResponse>>
    GetPendingActionsAsync()
        {
            var currentDate =
                DateTime.Now;

            var actions =
                new List<PendingActionResponse>();

            // Winner Pending

            var winnerPendingMatches =
                await _context.TournamentMatches
                    .Include(x => x.Tournament)
                    .Where(x =>
                        x.Player1Id != null &&
                        x.Player2Id != null &&
                        x.WinnerId == null &&
                        x.StartTime != null &&
                        x.StartTime < currentDate)
                    .ToListAsync();

            actions.AddRange(
                winnerPendingMatches.Select(x =>
                    new PendingActionResponse
                    {
                        MatchId = x.Id,

                        TournamentId =
                            x.TournamentId,

                        TournamentName =
                            x.Tournament.Name,

                        ActionType =
                            "WinnerPending",

                        Message =
                            $"Winner update pending for Match #{x.MatchNumber}",

                        MatchDate =
                            x.MatchDate
                    }));


            // Schedule Pending

            var schedulePendingMatches =
                await _context.TournamentMatches
                    .Include(x => x.Tournament)
                    .Include(x => x.Round)
                    .Where(x =>
                        x.Player1Id != null &&
                        x.Player2Id != null &&
                        x.MatchDate == null &&
                        x.WinnerId == null &&
                        x.Round.RoundNumber > 1)
                    .ToListAsync();

            actions.AddRange(
                schedulePendingMatches.Select(x =>
                    new PendingActionResponse
                    {
                        MatchId = x.Id,

                        TournamentId =
                            x.TournamentId,

                        TournamentName =
                            x.Tournament.Name,

                        ActionType =
                            "SchedulePending",

                        Message =
                            $"Match scheduling pending for Round {x.Round.RoundNumber}.",

                        MatchDate = null
                    }));


            // Tournament Winner Pending

            var tournamentWinnerPending =
     await _context.Tournaments
         .Include(x => x.TournamentMatches)
         .Include(x => x.TournamentRounds)
         .Where(t =>
             t.WinnerId == null &&
             t.TournamentMatches.Any(m =>
                 m.WinnerId != null &&
                 t.TournamentRounds.Any(r =>
                     r.Id == m.RoundId &&
                     r.RoundName == "Final")))
         .ToListAsync();

            actions.AddRange(
                tournamentWinnerPending.Select(x =>
                    new PendingActionResponse
                    {
                        MatchId = Guid.Empty,

                        TournamentId =
                            x.Id,

                        TournamentName =
                            x.Name,

                        ActionType =
                            "TournamentWinnerPending",

                        Message =
                            "Tournament winner declaration pending.",

                        MatchDate = null
                    }));

            return actions
                .OrderBy(x =>
                    x.MatchDate)
                .ToList();
        }

        public async Task<List<TournamentOverviewResponse>>
    GetTournamentOverviewAsync()
        {
            var currentDate =
                DateTime.Now;

            var tournaments =
                await _context.Tournaments
                    .Include(x => x.Game)
                    .Include(x => x.TournamentParticipants)
                    .Include(x => x.TournamentMatches)
                    .Where(x => !x.IsDeleted)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

            return tournaments
                .Select(x =>
                {
                    var totalMatches =
                        x.TournamentMatches.Count;

                    var completedMatches =
                        x.TournamentMatches
                            .Count(m =>
                                m.WinnerId != null);

                    int progress = 0;

                    if (totalMatches > 0)
                    {
                        progress =
                            (int)Math.Round(
                                (double)completedMatches
                                * 100 /
                                totalMatches);
                    }

                    string status =
                     TournamentStatusHelper.GetStatus(x);

                    return new TournamentOverviewResponse
                    {
                        TournamentId =
                            x.Id,

                        TournamentName =
                            x.Name,

                        GameName =
                            x.Game.Name,

                        Participants =
                            x.TournamentParticipants
                                .Count,

                        Status =
                            status,

                        TotalMatches =
                            totalMatches,

                        CompletedMatches =
                            completedMatches,

                        ProgressPercentage =
                            progress,

                        StartDate =
                            x.StartDate,

                        EndDate =
                            x.EndDate,
                        MatchProgress =
                            $"{completedMatches}/{totalMatches}"
                    };
                })
                .ToList();
        }

        public async Task<List<UpcomingMatchResponse>>
    GetUpcomingMatchesAsync()
        {
            var currentDate =
                DateTime.Now.Date;

            var matches =
                await _context.TournamentMatches
                    .Include(x => x.Tournament)
                    .Include(x => x.Round)
                    .Include(x => x.Player1)
                    .Include(x => x.Player2)
                    .Where(x =>
                        x.Player1Id != null &&
                        x.Player2Id != null &&
                        x.WinnerId == null &&
                        x.MatchDate != null &&
                        x.MatchDate >= currentDate)
                    .OrderBy(x =>
                        x.MatchDate)
                    .ThenBy(x =>
                        x.StartTime)
                    .Take(20)
                    .ToListAsync();

            return matches
                .Select(x =>
                    new UpcomingMatchResponse
                    {
                        MatchId =
                            x.Id,

                        TournamentId =
                            x.TournamentId,

                        TournamentName =
                            x.Tournament.Name,

                        Player1Name =
                            x.Player1?.FullName ?? "N / A",

                        Player2Name =
                            x.Player2?.FullName ?? "N /A",

                        RoundNumber =
                            x.Round.RoundNumber,

                        MatchDate =
                            x.MatchDate!.Value,

                        StartTime =
                            x.StartTime
                    })
                .ToList();
        }

        public async Task<List<TopPlayerResponse>>
    GetTopPlayersAsync()
        {
            var users =
                await _context.Users
                    .Where(x =>
                        !x.IsDeleted &&
                        x.IsActive &&
                        x.Role.Name ==
                            RoleConstants.User)
                    .ToListAsync();

            var response =
                new List<TopPlayerResponse>();

            foreach (var user in users)
            {
                var wins =
                    await _context.TournamentMatches
                        .CountAsync(x =>
                            x.WinnerId ==
                            user.Id);

                var matchesPlayed =
                    await _context.TournamentMatches
                        .CountAsync(x =>
                            x.WinnerId != null &&
                            (
                                x.Player1Id ==
                                    user.Id ||
                                x.Player2Id ==
                                    user.Id
                            ));

                var championships =
                    await _context.Tournaments
                        .CountAsync(x =>
                            x.WinnerId ==
                            user.Id);

                decimal winRate = 0;

                if (matchesPlayed > 0)
                {
                    winRate =
                        Math.Round(
                            (decimal)wins * 100 /
                            matchesPlayed,
                            2);
                }

                response.Add(
                    new TopPlayerResponse
                    {
                        UserId =
                            user.Id,

                        FullName =
                            user.FullName,

                        Wins =
                            wins,

                        Championships =
                            championships,

                        WinRate =
                            winRate
                    });
            }

            return response
                .OrderByDescending(x =>
                    x.Wins)
                .ThenByDescending(x =>
                    x.Championships)
                .Take(5)
                .ToList();
        }

        public async Task<List<RecentActivityResponse>>
    GetRecentActivitiesAsync()
        {
            var activities =
                new List<RecentActivityResponse>();

            // Tournament Winners

            var tournamentWinners =
                await _context.Tournaments
                    .Include(x => x.Winner)
                    .Where(x =>
                        x.WinnerId != null)
                    .OrderByDescending(x =>
                        x.UpdatedAt)
                    .Take(5)
                    .ToListAsync();

            activities.AddRange(
                tournamentWinners.Select(x =>
                    new RecentActivityResponse
                    {
                        ActivityType =
                            "TournamentWinner",

                        Message =
                            $"{x.Winner?.FullName ?? "N/A"} won {x.Name}.",

                        CreatedAt =
                            x.UpdatedAt ??
                            x.CreatedAt
                    }));


            // Match Winners

            var completedMatches =
                await _context.TournamentMatches
                    .Include(x => x.Winner)
                    .Include(x => x.Player1)
                    .Include(x => x.Player2)
                    .Include(x => x.Tournament)
                    .Where(x =>
                        x.WinnerId != null)
                    .OrderByDescending(x =>
                        x.CreatedAt)
                    .Take(10)
                    .ToListAsync();

            activities.AddRange(
                completedMatches.Select(x =>
                {
                    var loser =
                        x.Player1Id == x.WinnerId
                            ? x.Player2?.FullName ?? "N/A"
                            : x.Player1?.FullName ?? "N/A";

                    return new RecentActivityResponse
                    {
                        ActivityType =
                            "MatchCompleted",

                        Message =
                            $"{x.Winner?.FullName ?? "N/A"} defeated {loser} in {x.Tournament.Name}.",

                        CreatedAt =

                            x.CreatedAt
                    };
                }));


            // Recently Created Tournaments

            var recentTournaments =
                await _context.Tournaments
                    .OrderByDescending(x =>
                        x.CreatedAt)
                    .Take(5)
                    .ToListAsync();

            activities.AddRange(
                recentTournaments.Select(x =>
                    new RecentActivityResponse
                    {
                        ActivityType =
                            "TournamentCreated",

                        Message =
                            $"{x.Name} tournament created.",

                        CreatedAt =
                            x.CreatedAt
                    }));

            return activities
                .OrderByDescending(x =>
                    x.CreatedAt)
                .Take(10)
                .ToList();
        }


        public async Task<byte[]>
        ExportTournamentReportAsync(
            Guid tournamentId)
        {
            var tournament =
                await _context.Tournaments
                    .Include(x => x.Game)
                    .Include(x => x.Winner)
                    .Include(x => x.TournamentParticipants)
                        .ThenInclude(x => x.User)
                    .Include(x => x.TournamentMatches)
                        .ThenInclude(x => x.Player1)
                    .Include(x => x.TournamentMatches)
                        .ThenInclude(x => x.Player2)
                    .Include(x => x.TournamentMatches)
                        .ThenInclude(x => x.Winner)
                    .Include(x => x.TournamentMatches)
                        .ThenInclude(x => x.Round)
                    .FirstOrDefaultAsync(x =>
                        x.Id == tournamentId);

            if (tournament == null)
            {
                throw new ApiException(
                    "Tournament not found.",
                    StatusCodes.Status404NotFound);
            }

            if (tournament.WinnerId == null)
            {
                throw new ApiException(
                    "Tournament is not completed.",
                    StatusCodes.Status400BadRequest);
            }

            using var workbook =
                new XLWorkbook();

            // Summary Sheet

            var summary =
                workbook.Worksheets.Add(
                    "Summary");

            summary.Cell(1, 1).Value =
                "Tournament Name";

            summary.Cell(1, 2).Value =
                tournament.Name;

            summary.Cell(2, 1).Value =
                "Game";

            summary.Cell(2, 2).Value =
                tournament.Game.Name;

            summary.Cell(3, 1).Value =
                "Winner";

            summary.Cell(3, 2).Value =
                tournament.Winner?.FullName;

            summary.Cell(4, 1).Value =
                "Participants";

            summary.Cell(4, 2).Value =
                tournament.TournamentParticipants.Count;

            summary.Cell(5, 1).Value =
                "Total Matches";

            summary.Cell(5, 2).Value =
                tournament.TournamentMatches.Count;


            // Participants Sheet

            var participants =
                workbook.Worksheets.Add(
                    "Participants");

            participants.Cell(1, 1).Value =
                "Player";

            participants.Cell(1, 2).Value =
                "Email";

            int participantRow = 2;

            foreach (var player
                in tournament.TournamentParticipants)
            {
                participants.Cell(
                    participantRow,
                    1).Value =
                    player.User.FullName;

                participants.Cell(
                    participantRow,
                    2).Value =
                    player.User.Email;

                participantRow++;
            }


            // Match Results

            var matches =
                workbook.Worksheets.Add(
                    "Matches");

            matches.Cell(1, 1).Value =
                "Round";

            matches.Cell(1, 2).Value =
                "Player 1";

            matches.Cell(1, 3).Value =
                "Player 2";

            matches.Cell(1, 4).Value =
                "Winner";

            matches.Cell(1, 5).Value =
                "Score";



            int row = 2;

            foreach (var match
                in tournament.TournamentMatches
                    .OrderBy(x =>
                        x.Round.RoundNumber)
                    .ThenBy(x =>
                        x.MatchNumber))
            {
                matches.Cell(row, 1).Value =
                    match.Round.RoundName;

                matches.Cell(row, 2).Value =
                    match.Player1?.FullName;

                matches.Cell(row, 3).Value =
                    match.Player2?.FullName;

                matches.Cell(row, 4).Value =
                    match.Winner?.FullName;

                matches.Cell(row, 5).Value =
                    $"{match.Player1Score}-{match.Player2Score}";

                row++;
            }
            summary.Row(1).Style.Font.Bold = true;
            participants.Row(1).Style.Font.Bold = true;
            matches.Row(1).Style.Font.Bold = true;
            using var stream =
                new MemoryStream();
            matches.Columns().AdjustToContents();
            workbook.SaveAs(stream);
            await _logService.LogAsync(
                            level: "Information",
                        message: $"Downloaded summary of the tournament '{tournamentId}'.",
                        category: "Admin",
                        functionName: "DownloadExcelFile of summary",
                        moduleType: "API",
                        userId: Guid.Empty
                        );
            return stream.ToArray();
        }
    }
}