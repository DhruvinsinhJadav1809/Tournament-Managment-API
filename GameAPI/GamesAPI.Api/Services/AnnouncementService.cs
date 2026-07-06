using GamesAPI.Api.Constants;
using GamesAPI.Api.Data;
using GamesAPI.Api.DTOs;
using GamesAPI.Api.Exceptions;
using GamesAPI.Api.Hubs;
using GamesAPI.Api.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class AnnouncementService
    : IAnnouncementService
{
    private readonly IHubContext<AnnouncementHub>
    _hubContext;
    private readonly AppDbContext _context;

    public AnnouncementService(
        AppDbContext context,
        IHubContext<AnnouncementHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }
    private async Task ValidateRequestAsync(
    CreateAnnouncementRequest request)
    {

        if (!AnnouncementTypeConstants
        .All
        .Contains(request.Type))
        {
            throw new ApiException(
                "Invalid announcement type.",
                StatusCodes.Status400BadRequest);
        }

        if (!AnnouncementPriorityConstants
            .All
            .Contains(request.Priority))
        {
            throw new ApiException(
                "Invalid priority.",
                StatusCodes.Status400BadRequest);
        }

        if (!AnnouncementTargetTypeConstants
        .All
        .Contains(request.TargetType))

        {
            throw new ApiException(
                "Invalid target type.",
                StatusCodes.Status400BadRequest);
        }

        if (request.ExpireAt.HasValue &&
            request.ExpireAt.Value <
            DateTime.UtcNow)
        {
            throw new ApiException(
                "Expire date must be today or future.",
                StatusCodes.Status400BadRequest);
        }

        if (request.TargetType == AnnouncementTargetTypeConstants.Tournament)
        {
            if (!request.TournamentId
                    .HasValue)
            {
                throw new ApiException(
                    "TournamentId is required.",
                    StatusCodes.Status400BadRequest);
            }

            var tournamentExists =
                await _context.Tournaments
                    .AnyAsync(x =>
                        x.Id ==
                        request.TournamentId &&
                        !x.IsDeleted);

            if (!tournamentExists)
            {
                throw new ApiException(
                    "Tournament not found.",
                    StatusCodes.Status404NotFound);
            }
        }

        if (request.TargetType ==
            AnnouncementTargetTypeConstants.Match)
        {
            if (!request.MatchId
                    .HasValue)
            {
                throw new ApiException(
                    "MatchId is required.",
                    StatusCodes.Status400BadRequest);
            }

            var matchExists =
                await _context
                    .TournamentMatches
                    .AnyAsync(x =>
                        x.Id ==
                        request.MatchId);

            if (!matchExists)
            {
                throw new ApiException(
                    "Match not found.",
                    StatusCodes.Status404NotFound);
            }
        }

        if (request.TargetType ==
           AnnouncementTargetTypeConstants.User)
        {
            if (request.UserIds == null ||
                !request.UserIds.Any())
            {
                throw new ApiException(
                    "At least one user is required.",
                    StatusCodes.Status400BadRequest);
            }

            var userCount =
                await _context.Users
                    .CountAsync(x =>
                        request.UserIds
                            .Contains(x.Id)
                        &&
                        !x.IsDeleted);

            if (userCount !=
                request.UserIds.Count)
            {
                throw new ApiException(
                    "One or more users are invalid.",
                    StatusCodes.Status400BadRequest);
            }
        }
    }

    private async Task<List<Guid>>
    GetTargetUsersAsync(
        CreateAnnouncementRequest request)
    {
        switch (request.TargetType)
        {
            case AnnouncementTargetTypeConstants.AllUsers:

                return await _context.Users
                    .Where(x =>
                        !x.IsDeleted &&
                        x.IsActive)
                    .Select(x => x.Id)
                    .ToListAsync();

            case AnnouncementTargetTypeConstants.User:

                return request.UserIds!;

            case AnnouncementTargetTypeConstants.Tournament:

                return await _context
                    .TournamentParticipants
                    .Where(x =>
                        x.TournamentId ==
                        request.TournamentId)
                    .Select(x =>
                        x.UserId)
                    .Distinct()
                    .ToListAsync();

            case AnnouncementTargetTypeConstants.Match:

                var match =
                    await _context
                        .TournamentMatches
                        .Where(x =>
                            x.Id ==
                            request.MatchId)
                        .Select(x =>
                            new
                            {
                                x.Player1Id,
                                x.Player2Id
                            })
                        .FirstAsync();

                return new List<Guid>
            {
                match.Player1Id!.Value,
                match.Player2Id!.Value
            };

            default:

                return new List<Guid>();
        }
    }

    public async Task CreateAnnouncementAsync(
     Guid adminId,
     CreateAnnouncementRequest request)
    {
        await ValidateRequestAsync(request);

        var users =
            await GetTargetUsersAsync(request);

        var announcement =
            new Announcement
            {
                Id = Guid.NewGuid(),

                Title = request.Title.Trim(),

                Message = request.Message.Trim(),

                Type = request.Type,

                Priority = request.Priority,

                TargetType =
                    request.TargetType,

                TournamentId =
                    request.TournamentId,

                MatchId =
                    request.MatchId,

                ExpireAt =
                    request.ExpireAt,

                CreatedAt =
                    DateTime.UtcNow,

                CreatedByUserId =
                    adminId,

                IsActive = true
            };

        _context.Announcements.Add(
            announcement);

        foreach (var userId in users)
        {
            _context
                .AnnouncementRecipients
                .Add(
                    new AnnouncementRecipient
                    {
                        Id = Guid.NewGuid(),

                        AnnouncementId =
                            announcement.Id,

                        UserId = userId,

                        CreatedAt =
                            DateTime.UtcNow
                    });
        }

        await _context.SaveChangesAsync();

        var response =
    new AnnouncementSignalRResponse
    {
        Id = announcement.Id,

        Title = announcement.Title,

        Message = announcement.Message,

        Type = announcement.Type,

        Priority = announcement.Priority,

        CreatedAt =
            announcement.CreatedAt
    };

        foreach (var userId in users)
        {
            await _hubContext.Clients
                .Group(userId.ToString())
                .SendAsync(
                    "ReceiveAnnouncement",
                    response);
        }
    }

    public async Task<List<AnnouncementResponse>>
    GetUserAnnouncementsAsync(
        Guid userId)
    {
        var currentDate =
            DateTime.UtcNow;
        var isAdmin = await _context.Users
                .Where(x => x.Id == userId)
                .Select(x => x.Role.Name == RoleConstants.Admin)
                .FirstOrDefaultAsync();
        if (isAdmin)
        {
            return await _context.Announcements
        .Where(x =>
            !x.IsDeleted &&
            x.IsActive &&
            (
                x.ExpireAt == null ||
                x.ExpireAt >= currentDate
            ))
        .OrderByDescending(x => x.CreatedAt)
        .Select(x => new AnnouncementResponse
        {
            Id =
                        x.Id,

            Title =
                        x.Title,

            Message =
                        x.Message,

            Type =
                        x.Type,

            Priority =
                        x.Priority,

            IsRead =
                        false,

            CreatedAt =
                        x.CreatedAt,

            ExpireAt =
                        x.ExpireAt

        })
        .ToListAsync();
        }
        return await _context
            .AnnouncementRecipients
            .Where(x =>
                x.UserId == userId
                &&
                !x.Announcement.IsDeleted
                &&
                x.Announcement.IsActive
                &&
                (
                    x.Announcement.ExpireAt == null
                    ||
                    x.Announcement.ExpireAt >= currentDate
                ))
            .OrderByDescending(x =>
                x.Announcement.CreatedAt)
            .Select(x =>
                new AnnouncementResponse
                {
                    Id =
                        x.Announcement.Id,

                    Title =
                        x.Announcement.Title,

                    Message =
                        x.Announcement.Message,

                    Type =
                        x.Announcement.Type,

                    Priority =
                        x.Announcement.Priority,

                    IsRead =
                        x.IsRead,

                    CreatedAt =
                        x.Announcement.CreatedAt,

                    ExpireAt =
                        x.Announcement.ExpireAt
                })
            .ToListAsync();
    }

}