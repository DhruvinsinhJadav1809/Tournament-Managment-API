using GamesAPI.Api.Data;
using GamesAPI.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using GamesAPI.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using GamesAPI.Api.Constants;

namespace GamesAPI.Api.Services
{
    public class TournamentMatchesService : ITournamentMatchesService
    {
        private readonly AppDbContext _context;
        private readonly ICertificateService _certificateService;
        public TournamentMatchesService(
            AppDbContext context,
            ILogService logService,
            INotificationService notificationService,
            ICertificateService certificateService)
        {
            _context = context;
            _logService = logService;
            _notificationService =
            notificationService;
            _certificateService = certificateService;
        }
        private readonly
            ILogService
            _logService;
        private readonly INotificationService
            _notificationService;
        private int GetNextPowerOfTwo(
            int number)
        {
            int power = 1;

            while (power < number)
            {
                power *= 2;
            }

            return power;
        }
        private string GetRoundName(
        int roundNumber,
        int totalRounds)
        {
            if (roundNumber == totalRounds)
                return "Final";

            if (roundNumber ==
                totalRounds - 1)
                return "Semi Final";

            if (roundNumber ==
                totalRounds - 2)
                return "Quarter Final";

            return $"Round {roundNumber}";
        }

        private List<TournamentMatch>
    CreateBracketStructure(
        Guid tournamentId,
        List<TournamentRound> rounds,
        int bracketSize)
        {
            var matches =
                new List<TournamentMatch>();

            int matchNumber = 1;

            int matchesInRound =
                bracketSize / 2;

            foreach (var round in rounds)
            {
                for (int i = 0;
                     i < matchesInRound;
                     i++)
                {
                    matches.Add(
                        new TournamentMatch
                        {
                            Id =
                                Guid.NewGuid(),

                            TournamentId =
                                tournamentId,

                            RoundId =
                                round.Id,

                            MatchNumber =
                                matchNumber++,

                            Status =
                                "Pending"
                        });
                }

                matchesInRound /= 2;
            }

            return matches;
        }
        private void WireMatches(
            List<TournamentMatch> matches,
            List<TournamentRound> rounds)
        {
            for (int i = 1;
                 i < rounds.Count;
                 i++)
            {
                var previousRound =
                    rounds[i - 1];

                var currentRound =
                    rounds[i];

                var previousRoundMatches =
                    matches
                        .Where(x =>
                            x.RoundId ==
                            previousRound.Id)
                        .OrderBy(x =>
                            x.MatchNumber)
                        .ToList();

                var currentRoundMatches =
                    matches
                        .Where(x =>
                            x.RoundId ==
                            currentRound.Id)
                        .OrderBy(x =>
                            x.MatchNumber)
                        .ToList();

                int previousMatchIndex = 0;

                foreach (var match
                    in currentRoundMatches)
                {
                    match.PreviousMatch1Id =
                        previousRoundMatches[
                            previousMatchIndex]
                            .Id;

                    match.PreviousMatch2Id =
                        previousRoundMatches[
                            previousMatchIndex + 1]
                            .Id;

                    previousMatchIndex += 2;
                }
            }
        }
        private List<TournamentRound>
        CreateRounds(
        Guid tournamentId,
        int totalRounds)
        {
            var rounds =
                new List<TournamentRound>();

            for (int i = 1;
                 i <= totalRounds;
                 i++)
            {
                var round =
                    new TournamentRound
                    {
                        Id = Guid.NewGuid(),

                        TournamentId =
                            tournamentId,

                        RoundNumber = i,

                        RoundName =
                            GetRoundName(
                                i,
                                totalRounds),

                        Status = "Pending"
                    };

                rounds.Add(round);
            }

            return rounds;
        }
        private void
AssignRoundOnePlayers(
    List<TournamentMatch> matches,
    TournamentRound roundOne,
    List<TournamentParticipant> participants,
    int bracketSize)
        {
            var roundOneMatches =
                matches
                    .Where(x =>
                        x.RoundId ==
                        roundOne.Id)
                    .OrderBy(x =>
                        x.MatchNumber)
                    .ToList();

            int participantIndex = 0;

            foreach (var match
                in roundOneMatches)
            {
                if (participantIndex <
                    participants.Count)
                {
                    match.Player1Id =
                        participants[
                            participantIndex]
                            .UserId;

                    participantIndex++;
                }

                if (participantIndex <
                    participants.Count)
                {
                    match.Player2Id =
                        participants[
                            participantIndex]
                            .UserId;

                    participantIndex++;
                }

                if (match.Player2Id == null)
                {
                    match.IsBye = true;

                    match.WinnerId =
                        match.Player1Id;

                    match.Status =
                        "Completed";
                    match.Player1Score = 1;
                    match.Player2Score = 0;
                }
            }
        }
        private void ScheduleRoundOneMatches(
            List<TournamentMatch> matches,
            TournamentRound roundOne,
            GenerateMatchesRequest request)
        {
            var roundOneMatches =
                matches
                    .Where(x =>
                        x.RoundId ==
                        roundOne.Id)
                    .OrderBy(x =>
                        x.MatchNumber)
                    .ToList();

            var currentDate =
                request.StartDate.Date;

            var currentTime =
                request.FirstMatchTime;

            int matchesToday = 0;

            foreach (var match
                in roundOneMatches)
            {
                if (matchesToday ==
                    request.MatchesPerDay)
                {
                    currentDate =
                        currentDate.AddDays(1);

                    currentTime =
                        request.FirstMatchTime;

                    matchesToday = 0;
                }

                var startDateTime =
                    currentDate
                        .Add(currentTime);

                var endDateTime =
                    startDateTime
                        .AddMinutes(
                            request.MatchDurationMinutes);

                match.MatchDate =
                    currentDate;

                match.StartTime =
                    startDateTime;

                match.EndTime =
                    endDateTime;

                currentTime =
                    endDateTime.TimeOfDay
                        .Add(
                            TimeSpan.FromMinutes(
                                request.BreakMinutes));

                matchesToday++;

            }

        }
        private void AdvanceCompletedMatches(
            List<TournamentMatch> matches)
        {
            var completedMatches =
                matches
                    .Where(x =>
                        x.WinnerId.HasValue)
                    .ToList();

            foreach (var completedMatch
                in completedMatches)
            {
                var nextMatch =
                    matches
                        .FirstOrDefault(x =>
                            x.PreviousMatch1Id ==
                                completedMatch.Id ||
                            x.PreviousMatch2Id ==
                                completedMatch.Id);

                if (nextMatch == null)
                {
                    continue;
                }

                if (nextMatch.PreviousMatch1Id ==
                    completedMatch.Id)
                {
                    nextMatch.Player1Id =
                        completedMatch.WinnerId;
                }
                else
                {
                    nextMatch.Player2Id =
                        completedMatch.WinnerId;
                }
            }
        }
        private async Task CreateRoundOneNotificationsAsync(
    List<TournamentMatch> matches)
        {
            var roundOneMatches =
                matches
                    .Where(x =>
                        x.Player1Id.HasValue &&
                        x.Player2Id.HasValue);

            foreach (var match in roundOneMatches)
            {
                await _notificationService
                    .CreateNotificationAsync(
                        match.Player1Id!.Value,
                        "Match Scheduled",
                        $"Your match is scheduled on {match.StartTime:dd-MMM-yyyy hh:mm tt}.",
                        NotificationTypes.Action);

                await _notificationService
                    .CreateNotificationAsync(
                        match.Player2Id!.Value,
                        "Match Scheduled",
                        $"Your match is scheduled on {match.StartTime:dd-MMM-yyyy hh:mm tt}.",
                        NotificationTypes.Action);
            }
        }
        public async Task GenerateMatchesAsync(
         Guid tournamentId,
             GenerateMatchesRequest request)
        {
            var currentDate =
                DateTime.Now;
            var tournament =
            await _context.Tournaments
                .FirstOrDefaultAsync(x =>
            x.Id == tournamentId &&
            !x.IsDeleted);

            if (tournament == null)
            {
                throw new NotFoundException(
                    "Tournament not found.");
            }
            if (!tournament.IsActive)
            {
                throw new BadRequestException(
                    "Tournament is inactive.");
            }
            if (currentDate <=
                tournament.RegistrationEndDate)
            {
                throw new BadRequestException(
                    "Registration is still open.");
            }
            if (currentDate >=
            tournament.StartDate)
            {
                throw new BadRequestException(
                    "Tournament has already started.");
            }
            var matchesExist =
            await _context.TournamentMatches
                .AnyAsync(x =>
                    x.TournamentId ==
                    tournamentId);

            if (matchesExist)
            {
                throw new BadRequestException(
                    "Matches have already been generated.");
            }
            var participants =
                await _context
                    .TournamentParticipants
                    .Where(x =>
                        x.TournamentId ==
                            tournamentId &&
                        !x.IsWithdrawn)
                    .ToListAsync();

            if (participants.Count < 2)
            {
                throw new BadRequestException(
                    "At least two participants are required.");
            }
            if (request.MatchesPerDay <= 0)
            {
                throw new BadRequestException(
                    "Matches per day must be greater than zero.");
            }
            if (request.MatchDurationMinutes <= 0)
            {
                throw new BadRequestException(
                    "Match duration must be greater than zero.");
            }
            if (request.BreakMinutes < 0)
            {
                throw new BadRequestException(
                    "Break minutes cannot be negative.");
            }
            if (request.StartDate.Date <
                currentDate.Date)
            {
                throw new BadRequestException(
                    "Round start date cannot be in the past.");
            }

            var random =
                    new Random();

            var shuffledParticipants =
                participants
                    .OrderBy(x =>
                        random.Next())
                    .ToList();

            var bracketSize =
                GetNextPowerOfTwo(
                    participants.Count);

            var totalRounds =
                (int)Math.Log2(
                    bracketSize);

            var rounds =
                CreateRounds(
                    tournamentId,
                    totalRounds);

            await _context
                .TournamentRounds
                .AddRangeAsync(
                    rounds);

            var matches =
                CreateBracketStructure(
                    tournamentId,
                    rounds,
                    bracketSize);
            WireMatches(
                matches,
                rounds);
            AssignRoundOnePlayers(
                matches,
                rounds.First(),
                shuffledParticipants,
                bracketSize);
            AdvanceCompletedMatches(
                matches);
            ScheduleRoundOneMatches(
                matches,
                rounds.First(),
                request);
            await CreateRoundOneNotificationsAsync(
                matches);
            await _context
                .TournamentMatches
                .AddRangeAsync(
                    matches);

            await _context
                .SaveChangesAsync();


        }


        public async Task<TournamentMatchesResponse>
            GetTournamentMatchesAsync(
                Guid tournamentId)
        {
            var tournament =
                await _context.Tournaments
                    .FirstOrDefaultAsync(x =>
                        x.Id == tournamentId &&
                        !x.IsDeleted);

            if (tournament == null)
            {
                throw new NotFoundException(
                    "Tournament not found.");
            }

            var rounds =
                await _context.TournamentRounds
                    .Where(x =>
                        x.TournamentId ==
                        tournamentId)
                    .OrderBy(x =>
                        x.RoundNumber)
                    .ToListAsync();

            var matches =
                await _context.TournamentMatches
                    .Include(x => x.Player1)
                    .Include(x => x.Player2)
                    .Include(x => x.Winner)
                    .Where(x =>
                        x.TournamentId ==
                        tournamentId)
                    .OrderBy(x =>
                        x.MatchNumber)
                    .ToListAsync();

            var response =
                new TournamentMatchesResponse
                {
                    TournamentId =
                        tournament.Id,

                    TournamentName =
                        tournament.Name,

                    Rounds =
                        rounds.Select(round =>
                            new RoundResponse
                            {
                                RoundId =
                                    round.Id,

                                RoundNumber =
                                    round.RoundNumber,

                                RoundName =
                                    round.RoundName,

                                Matches =
                                    matches
                                        .Where(x =>
                                            x.RoundId ==
                                            round.Id)
                                        .Select(match =>
                                            new MatchResponse
                                            {
                                                MatchId =
                                                    match.Id,

                                                MatchNumber =
                                                    match.MatchNumber,

                                                Player1Id =
                                                    match.Player1Id,

                                                Player1Name =
                                                    match.Player1
                                                        ?.FullName,

                                                Player2Id =
                                                    match.Player2Id,

                                                Player2Name =
                                                    match.Player2
                                                        ?.FullName,

                                                WinnerId =
                                                    match.WinnerId,

                                                Status =
                                                    match.Status,

                                                MatchDate =
                                                    match.MatchDate,

                                                StartTime =
                                                    match.StartTime,

                                                EndTime =
                                                    match.EndTime,

                                                IsBye =
                                                    match.IsBye,
                                                Player1Score = match.Player1Score,
                                                Player2Score = match.Player2Score
                                            })
                                        .OrderBy(x =>
                                            x.MatchNumber)
                                        .ToList()
                            })
                        .ToList()
                };

            return response;
        }

        public async Task UpdateMatchResultAsync(
          Guid matchId,
          UpdateMatchResultRequest request)
        {
            var match =
                await _context.TournamentMatches
                    .Include(x => x.Round)
                    .FirstOrDefaultAsync(x =>
                        x.Id == matchId);

            if (match == null)
            {
                throw new NotFoundException(
                    "Match not found.");
            }

            if (match.Status == "Completed")
            {
                throw new BadRequestException(
                    "Match is already completed.");
            }

            if (!match.Player1Id.HasValue ||
                !match.Player2Id.HasValue)
            {
                throw new BadRequestException(
                    "Match is not ready.");
            }

            if (request.Player1Score ==
                request.Player2Score)
            {
                throw new BadRequestException(
                    "Draw is not allowed.");
            }

            if (request.WinnerId != match.Player1Id &&
                request.WinnerId != match.Player2Id)
            {
                throw new BadRequestException(
                    "Winner must be one of the match players.");
            }

            if (request.Player1Score >
                request.Player2Score &&
                request.WinnerId != match.Player1Id)
            {
                throw new BadRequestException(
                    "Winner does not match score.");
            }

            if (request.Player2Score >
                request.Player1Score &&
                request.WinnerId != match.Player2Id)
            {
                throw new BadRequestException(
                    "Winner does not match score.");
            }

            match.Player1Score =
                request.Player1Score;

            match.Player2Score =
                request.Player2Score;

            match.WinnerId =
                request.WinnerId;

            match.Status =
                "Completed";

            var nextMatch =
                await _context.TournamentMatches
                    .FirstOrDefaultAsync(x =>
                        x.PreviousMatch1Id == match.Id ||
                        x.PreviousMatch2Id == match.Id);

            if (nextMatch != null)
            {
                if (nextMatch.PreviousMatch1Id ==
                    match.Id)
                {
                    nextMatch.Player1Id =
                        request.WinnerId;
                }

                if (nextMatch.PreviousMatch2Id ==
                    match.Id)
                {
                    nextMatch.Player2Id =
                        request.WinnerId;
                }

                if (nextMatch.Player1Id.HasValue &&
                    nextMatch.Player2Id.HasValue)
                {
                    nextMatch.Status =
                        "Ready";
                }
                await _notificationService
    .CreateNotificationAsync(
        request.WinnerId,
        "Advanced To Next Round",
        $"You advanced to {nextMatch.Round}.",
        NotificationTypes.Information);
            }
            else
            {
                var tournament =
                    await _context.Tournaments
                        .FirstOrDefaultAsync(x =>
                            x.Id ==
                            match.TournamentId);

                if (tournament != null)
                {
                    tournament.WinnerId =
                        request.WinnerId;
                    await _notificationService
                    .CreateNotificationAsync(
                        request.WinnerId,
                        "Tournament Champion",
                        $"Congratulations! You won {tournament.Name}.",
                        NotificationTypes.Information);

                }
            }

            await _context.SaveChangesAsync();
            if (nextMatch == null)
            {
                await _certificateService
                    .SendWinnerCertificateAsync(match.TournamentId);
            }
            await _notificationService
    .CreateNotificationAsync(
        request.WinnerId,
        "Match Won",
        "Congratulations! You won your match.",
        NotificationTypes.Information);
            await _notificationService
        .CreateNotificationAsync(
           request.WinnerId == match.Player1Id.Value ? match.Player2Id.Value : match.Player1Id.Value,
            "Match Lost",
            "Your tournament journey has ended.",
            NotificationTypes.Information);
        }

        public async Task UpdateMatchScheduleAsync(
    Guid matchId,
    UpdateMatchScheduleRequest request)
        {
            var match =
                await _context.TournamentMatches
                    .Include(x => x.Round)
                    .FirstOrDefaultAsync(x =>
                        x.Id == matchId);

            if (match == null)
            {
                throw new NotFoundException(
                    "Match not found.");
            }

            if (match.Round.RoundNumber == 1)
            {
                throw new BadRequestException(
                    "Round 1 schedule cannot be modified.");
            }

            if (match.Status != "Ready")
            {
                throw new BadRequestException(
                    "Only ready matches can be scheduled.");
            }

            if (request.StartTime >=
                request.EndTime)
            {
                throw new BadRequestException(
                    "End time must be greater than start time.");
            }

            if (request.MatchDate <
                DateTime.Now.Date)
            {
                throw new BadRequestException(
                    "Match date cannot be in the past.");
            }

            match.MatchDate =
                request.MatchDate;

            match.StartTime =
                request.StartTime;

            match.EndTime =
                request.EndTime;

            match.Status =
                "Scheduled";

            await _context.SaveChangesAsync();
            await _notificationService
    .CreateNotificationAsync(
        userId: match.Player1Id.HasValue ? match.Player1Id.Value : Guid.Empty,
        title: "Match Scheduled",
        message:
            $"Your match is scheduled on {match.MatchDate:dd-MMM-yyyy hh:mm tt}.",
        type:
            NotificationTypes.Action);
            await _notificationService
    .CreateNotificationAsync(
        match.Player2Id.HasValue ? match.Player2Id.Value : Guid.Empty,
        "Match Scheduled",
        $"Your match is scheduled on {match.MatchDate:dd-MMM-yyyy hh:mm tt}.",
        NotificationTypes.Action);
        }

        public async Task<List<UpcomingMatchResponse>>
    GetUpcomingMatchesAsync(
        Guid userId)
        {
            var matches =
                await _context.TournamentMatches
                    .Include(x => x.Tournament)
                    .Include(x => x.Round)
                    .Include(x => x.Player1)
                    .Include(x => x.Player2)
                    .Where(x =>
                        (x.Player1Id == userId ||
                         x.Player2Id == userId) &&
                        (x.Status == "Ready" ||
                         x.Status == "Scheduled"))
                    .OrderBy(x => x.MatchDate)
                    .ThenBy(x => x.StartTime)
                    .Select(x =>
                        new UpcomingMatchResponse
                        {
                            MatchId = x.Id,

                            TournamentId =
                                x.TournamentId,

                            TournamentName =
                                x.Tournament.Name,

                            RoundName =
                                x.Round.RoundName,


                            OpponentName = x.Player1Id == userId
                            ? (x.Player2 != null ? x.Player2.FullName : "TBD")
                            : (x.Player1 != null ? x.Player1.FullName : "TBD"),

                            Status =
                                x.Status,

                            MatchDate =
                                x.MatchDate,

                            StartTime =
                                x.StartTime,

                            EndTime =
                                x.EndTime
                        })
                    .ToListAsync();

            return matches;
        }
    }
}