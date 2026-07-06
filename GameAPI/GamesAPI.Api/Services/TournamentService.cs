using GamesAPI.Api.Data;
using GamesAPI.Api.DTOs.Tournaments;
using GamesAPI.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using GamesAPI.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using GamesAPI.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GamesAPI.Api.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly AppDbContext _context;

        public TournamentService(
            AppDbContext context,
            ILogService logService)
        {
            _context = context;
            _logService = logService;
        }
        private readonly
            ILogService
            _logService;


        public async Task<GetTournamentsResponse>
            GetTournamentsAsync(
            GetTournamentsRequest request)
        {
            var query = _context.Tournaments
                        .Include(x => x.Game)
                        .Where(x => !x.IsDeleted)
                        .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(
                request.Search))
            {
                query = query.Where(x =>
                    x.Name.Contains(
                        request.Search));
            }

            // Active filter
            if (request.IsActive.HasValue)
            {
                query = query.Where(x =>
                    x.IsActive ==
                    request.IsActive.Value);
            }
            if (request.GameId.HasValue)
            {
                query = query.Where(x =>
                    x.GameId == request.GameId.Value);
            }
            if (request.StartDate.HasValue)
            {
                query = query.Where(x =>
                    x.StartDate >= request.StartDate.Value);
            }
            if (request.EndDate.HasValue)
            {
                query = query.Where(x =>
                    x.EndDate <= request.EndDate.Value);
            }
            var currentDate = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                switch (request.Status.Trim().ToLower())
                {
                    case "upcoming":
                        query = query.Where(x =>
                            currentDate < x.RegistrationStartDate);
                        break;

                    case "registrationopen":
                        query = query.Where(x =>
                            currentDate >= x.RegistrationStartDate &&
                            currentDate <= x.RegistrationEndDate);
                        break;

                    case "registrationclosed":
                        query = query.Where(x =>
                            currentDate > x.RegistrationEndDate &&
                            currentDate < x.StartDate);
                        break;

                    case "ongoing":
                        query = query.Where(x =>
                            currentDate >= x.StartDate &&
                            currentDate <= x.EndDate);
                        break;

                    case "completed":
                        query = query.Where(x =>
                            currentDate > x.EndDate);
                        break;
                }
            }

            // Total count
            var totalCount =
                await query.CountAsync();

            // Pagination
            var tournaments = await query
                            .Skip(
                                (request.Page - 1)
                                * request.PageSize)
                            .Take(request.PageSize)
                            .Select(x => new TournamentResponse
                            {
                                Id = x.Id,
                                Name = x.Name,
                                GameId = x.GameId,
                                GameName = x.Game.Name,
                                TournamentType =
                                    x.TournamentType,
                                MaxParticipants =
                                    x.MaxParticipants,
                                StartDate =
                                    x.StartDate,
                                EndDate =
                                    x.EndDate,
                                RegistrationEndDate = x.RegistrationEndDate,
                                RegistrationStartDate = x.RegistrationStartDate,
                                IsActive =
                                    x.IsActive,
                                StatusName =
                                    TournamentStatusHelper.GetStatus(x),
                                CurrentParticipants =
                                    _context.TournamentParticipants
                                        .Count(tp =>
                                            tp.TournamentId == x.Id &&
                                            !tp.IsWithdrawn),
                                IsGeneratedMatches =
                            _context.TournamentMatches
                                .Any(m =>
                                    m.TournamentId ==
                                    x.Id)
                            })
                            .ToListAsync();

            return new GetTournamentsResponse
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = tournaments
            };
        }

        public async Task<Tournament>
        CreateTournamentAsync(
            CreateTournamentRequest request,
            Guid userId)
        {
            if (request.RegistrationStartDate <
                DateTime.Now.Date)
            {
                throw new BadRequestException(
                    "Registration start date cannot be in the past.");
            }

            if (request.RegistrationStartDate >=
            request.RegistrationEndDate)
            {
                throw new BadRequestException(
                    "Registration end date must be greater than registration start date.");
            }

            if (request.RegistrationEndDate >=
                request.StartDate)
            {
                throw new BadRequestException(
                    "Tournament start date must be greater than registration end date.");
            }

            if (request.StartDate >=
                request.EndDate)
            {
                throw new BadRequestException(
                    "Tournament end date must be greater than tournament start date.");
            }
            // validations
            var gameExists =
                await _context.Games
                    .AnyAsync(x =>
                        x.Id == request.GameId!.Value
                        && !x.IsDeleted);

            if (!gameExists)
            {
                throw new BadRequestException(
                    "Selected game does not exist.");
            }

            var tournament =
                new Tournament
                {
                    Id = Guid.NewGuid(),

                    Name =
                        request.Name.Trim(),

                    GameId =
                        request.GameId!.Value,

                    TournamentType =
                        request.TournamentType,

                    MaxParticipants =
                        request.MaxParticipants,

                    StartDate =
                        request.StartDate,

                    EndDate =
                        request.EndDate,
                    RegistrationStartDate = request.RegistrationStartDate,
                    RegistrationEndDate = request.RegistrationEndDate,
                    ParticipantsPerEntry = request.ParticipantsPerEntry,
                    Description =
                        request.Description,


                    IsActive =
                        request.IsActive,

                    IsDeleted =
                        false,

                    CreatedAt =
                        DateTime.Now,

                    CreatedByUserId =
                        userId
                };

            _context.Tournaments
                .Add(tournament);

            await _context
                .SaveChangesAsync();

            return tournament;
        }

        public async Task<TournamentResponse>
         GetTournamentByIdAsync(Guid id)
        {
            var currentDate = DateTime.Now;
            var tournament =
                await _context.Tournaments
                    .Include(x => x.Game)
                    .Where(x => x.Id == id
                        && !x.IsDeleted)
                    .Select(x => new TournamentResponse
                    {
                        Id = x.Id,
                        Name = x.Name,
                        GameId = x.GameId,
                        GameName = x.Game.Name,
                        TournamentType =
                            x.TournamentType,
                        MaxParticipants =
                            x.MaxParticipants,
                        StartDate =
                            x.StartDate,
                        EndDate =
                            x.EndDate,

                        IsActive =
                            x.IsActive,
                        StatusName = TournamentStatusHelper.GetStatus(x),
                        CurrentParticipants =
                            _context.TournamentParticipants
                                .Count(tp =>
                                    tp.TournamentId == x.Id &&
                                    !tp.IsWithdrawn),
                    })
                    .FirstOrDefaultAsync();

            if (tournament == null)
            {
                return null;
            }

            return tournament;
        }

        public async Task<bool> DeleteTournamentAsync(Guid id, Guid userId)
        {
            var tournament =
                await _context.Tournaments
                    .FirstOrDefaultAsync(x =>
                        x.Id == id
                        && !x.IsDeleted);

            if (tournament == null)
            {
                return false;
            }

            tournament.IsDeleted = true;
            tournament.DeletedAt = DateTime.Now;
            tournament.updatedByUserId = userId;
            tournament.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Tournament?>
         UpdateTournamentAsync(Guid id, UpdateTournamentRequest request, Guid updateByUserId)
        {
            if (request.RegistrationStartDate >=
                request.RegistrationEndDate)
            {
                throw new BadRequestException(
                    "Registration end date must be greater than registration start date.");
            }

            if (request.RegistrationEndDate >=
                request.StartDate)
            {
                throw new BadRequestException(
                    "Tournament start date must be greater than registration end date.");
            }

            if (request.StartDate >=
                request.EndDate)
            {
                throw new BadRequestException(
                    "Tournament end date must be greater than tournament start date.");
            }
            var tournament =
                await _context.Tournaments
                    .FirstOrDefaultAsync(x =>
                        x.Id == id
                        && !x.IsDeleted);

            if (tournament == null)
            {
                return null;
            }

            // validations
            var gameExists =
                await _context.Games
                    .AnyAsync(x =>
                        x.Id == request.GameId!.Value
                        && !x.IsDeleted);

            if (!gameExists)
            {
                throw new BadRequestException(
                    "Selected game does not exist.");
            }


            tournament.Name = request.Name.Trim();
            tournament.GameId = request.GameId!.Value;
            tournament.TournamentType = request.TournamentType;
            tournament.MaxParticipants = request.MaxParticipants;
            tournament.StartDate = request.StartDate;
            tournament.EndDate = request.EndDate;
            tournament.Description = request.Description;
            tournament.IsActive = request.IsActive;
            tournament.UpdatedAt = DateTime.Now;
            tournament.updatedByUserId = updateByUserId;
            tournament.RegistrationStartDate = request.RegistrationStartDate;
            tournament.RegistrationEndDate = request.RegistrationEndDate;
            tournament.ParticipantsPerEntry = request.ParticipantsPerEntry;
            await _context.SaveChangesAsync();

            return tournament;
        }


        public async Task
          JoinTournamentAsync(
            Guid tournamentId,
            Guid userId)
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
            if (!tournament.IsActive)
            {
                throw new BadRequestException(
                    "Tournament is inactive.");
            }
            var currentDate =
                DateTime.Now;

            if (currentDate < tournament.RegistrationStartDate ||
                currentDate > tournament.RegistrationEndDate)
            {
                throw new BadRequestException(
                    "Registration window is closed.");
            }
            var participantCount =
                await _context.TournamentParticipants
                    .CountAsync(x =>
                        x.TournamentId == tournamentId &&
                        !x.IsWithdrawn);

            if (participantCount >=
                tournament.MaxParticipants)
            {
                throw new BadRequestException(
                    "Tournament is full.");
            }
            var existingParticipant =
            await _context.TournamentParticipants
                .FirstOrDefaultAsync(x =>
                    x.TournamentId == tournamentId &&
                    x.UserId == userId);
            if (existingParticipant == null)
            {
                var participant =
                    new TournamentParticipant
                    {
                        Id = Guid.NewGuid(),
                        TournamentId = tournamentId,
                        UserId = userId,
                        JoinedAt = DateTime.Now,
                        IsWithdrawn = false
                    };

                await _context.TournamentParticipants
                    .AddAsync(participant);
            }
            else if (!existingParticipant.IsWithdrawn)
            {
                throw new BadRequestException(
                    "You have already joined this tournament.");
            }
            else
            {
                existingParticipant.IsWithdrawn = false;
                existingParticipant.WithdrawnAt = null;
                existingParticipant.JoinedAt = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            await _logService.LogAsync(
                level: "Information",
                message: "User joined tournament.",
                category: "Tournament",
                functionName: "JoinTournamentAsync",
                userId: userId,
                moduleType: "Tournament",
                page: "Join Tournament");
        }

        public async Task<List<TournamentDashboardResponse>>
    GetDashboardTournamentsAsync(Guid userId)
        {
            var currentDate =
                DateTime.Now;
            var joinedTournamentIds =
                await _context.TournamentParticipants
                    .Where(x =>
                        x.UserId == userId &&
                        !x.IsWithdrawn)
                    .Select(x => x.TournamentId)
                    .ToListAsync();
            var tournaments =
                await _context.Tournaments
                    .Include(x => x.Game)
                    .Where(x =>
                        !x.IsDeleted &&
                        x.IsActive)
                    .Select(x => new TournamentDashboardResponse
                    {
                        Id = x.Id,

                        Name = x.Name,

                        GameName = x.Game.Name,

                        TournamentType =
                            x.TournamentType,

                        MaxParticipants =
                            x.MaxParticipants,

                        CurrentParticipants =
                            _context.TournamentParticipants
                                .Count(tp =>
                                    tp.TournamentId == x.Id &&
                                    !tp.IsWithdrawn),

                        RegistrationStartDate =
                            x.RegistrationStartDate,

                        RegistrationEndDate =
                            x.RegistrationEndDate,

                        StartDate =
                            x.StartDate,

                        Status =
                            TournamentStatusHelper.GetStatus(x),
                        IsParticipated =
                                joinedTournamentIds.Contains(x.Id),

                    })
                    .ToListAsync();

            return tournaments
                .Where(x =>
                    x.Status != "Completed")
                .ToList();
        }


        public async Task<TournamentParticipantsResponse>
    GetTournamentParticipantsAsync(
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

            var participants =
                await _context.TournamentParticipants
                    .Include(x => x.User)
                    .Where(x =>
                        x.TournamentId == tournamentId &&
                        !x.IsWithdrawn)
                    .Select(x =>
                        new TournamentParticipantResponse
                        {
                            UserId = x.UserId,

                            FullName =
                                x.User.FullName,

                            Email =
                                x.User.Email,

                            JoinedAt =
                                x.JoinedAt
                        })
                    .OrderBy(x => x.JoinedAt)
                    .ToListAsync();

            return new TournamentParticipantsResponse
            {
                TournamentId =
            tournament.Id,

                TournamentName =
            tournament.Name,

                MaxParticipants =
            tournament.MaxParticipants,

                CurrentParticipants =
            participants.Count,

                Participants =
            participants
            };
        }



        public async Task<GetMyTournamentsResponse>
            GetMyTournamentsAsync(
                Guid userId)
        {
            var currentDate = DateTime.Now;
            var tournaments =
                await _context
                    .TournamentParticipants
                    .Include(x => x.Tournament)
                        .ThenInclude(x => x.Game)
                    .Where(x =>
                        x.UserId == userId &&
                        !x.IsWithdrawn)
                    .Select(x =>
                        new MyTournamentResponse
                        {
                            TournamentId =
                                x.Tournament.Id,

                            TournamentName =
                                x.Tournament.Name,

                            GameName =
                                x.Tournament.Game.Name,

                            Status =
                            TournamentStatusHelper.GetStatus(x.Tournament),

                            StartDate =
                                x.Tournament.StartDate,

                            EndDate =
                                x.Tournament.EndDate,

                            IsGeneratedMatches =
                                _context.TournamentMatches
                                    .Any(m =>
                                        m.TournamentId ==
                                        x.Tournament.Id),

                            IsChampion =
                                x.Tournament.WinnerId ==
                                userId
                        })
                    .ToListAsync();

            var response =
                new GetMyTournamentsResponse
                {
                    ActiveTournaments =
                        tournaments
                            .Where(x =>
                                x.Status !=
                                "Completed")
                            .ToList(),

                    CompletedTournaments =
                        tournaments
                            .Where(x =>
                                x.Status ==
                                "Completed")
                            .ToList()
                };

            return response;
        }
    }
}
