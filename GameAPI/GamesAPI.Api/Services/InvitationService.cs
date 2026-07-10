namespace GamesAPI.Api.Services;

using GamesAPI.Api.Data;
using GamesAPI.Api.DTOs.Games;
using GamesAPI.Api.DTOs.User;
using GamesAPI.Api.Enums;
using GamesAPI.Api.Exceptions;
using GamesAPI.Api.Interfaces;
using GamesAPI.Api.Models;
using Microsoft.EntityFrameworkCore;
public class InvitationService
    : IInvitationService
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IConfiguration
               _configuration;
    private readonly IUserService _userService;
    public InvitationService(
        AppDbContext context,
        IEmailService emailService, IConfiguration configuration,
        IUserService userService)
    {
        _context = context;
        _emailService = emailService;
        _configuration =
               configuration;
        _userService = userService;

    }

    public async Task InviteUserAsync(
       Guid currentUserId,
       InviteUserRequest request)
    {
        request.Email = request.Email.Trim().ToLower();

        // 1. Already Registered?
        var userExists = await _context.Users
            .AnyAsync(x =>
                x.Email.ToLower() == request.Email &&
                !x.IsDeleted);

        if (userExists)
        {
            throw new ApiException(
                "User is already registered.",
                StatusCodes.Status400BadRequest);
        }

        // 2. Already Invited?
        var invitationExists = await _context.Invitations
            .AnyAsync(x =>
                x.Email.ToLower() == request.Email &&
                x.Status == InvitationStatus.Pending &&
                x.ExpiresAt > DateTime.Now &&
                !x.IsDeleted);

        if (invitationExists)
        {
            throw new ApiException(
                "Invitation already sent.",
                StatusCodes.Status400BadRequest);
        }

        // 3. Get Inviter
        var invitedByUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == currentUserId);

        if (invitedByUser == null)
        {
            throw new ApiException(
                "Inviting user not found.",
                StatusCodes.Status404NotFound);
        }

        // 4. Generate Token
        var token = Guid.NewGuid();

        // 5. Create Invitation
        var invitation = new Invitation
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            Token = token,
            InvitedByUserId = currentUserId,
            Status = InvitationStatus.Pending,
            ExpiresAt = DateTime.Now.AddDays(7),
            CreatedAt = DateTime.Now,
            IsDeleted = false
        };

        _context.Invitations.Add(invitation);

        await _context.SaveChangesAsync();

        // 6. Read HTML Template
        var templatePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            EmailTemplates.InviteUser);

        var html = await File.ReadAllTextAsync(templatePath);
        var baseURL =
                       _configuration[
                           "FrontendSettings:BaseUrl"];
        // 7. Replace placeholders
        html = html.Replace("{{UserName}}", invitation.FullName);

        html = html.Replace("{{InvitedBy}}", invitedByUser.FullName);

        html = html.Replace("{{TournamentName}}", "Tournament Organizer");

        html = html.Replace(
            "{{InviteLink}}",
            $"{baseURL}/invite/{invitation.Token}");

        html = html.Replace(
            "{{ExpiryDate}}",
            invitation.ExpiresAt.ToString("dd MMM yyyy"));

        // 8. Send Email
        await _emailService.SendEmailAsync(
            invitation.Email,
            "You're invited to Tournament Organizer 🎉",
            html);
    }

    public async Task<InvitationDetailsResponse> GetInvitationAsync(
    Guid token)
    {
        var invitation = await _context.Invitations
            .FirstOrDefaultAsync(x =>
                x.Token == token &&
                !x.IsDeleted);

        if (invitation == null)
        {
            throw new ApiException(
                "Invitation not found.",
                StatusCodes.Status404NotFound);
        }

        if (invitation.Status == InvitationStatus.Accepted)
        {
            throw new ApiException(
                "Invitation has already been accepted.",
                StatusCodes.Status400BadRequest);
        }

        if (invitation.Status == InvitationStatus.Cancelled)
        {
            throw new ApiException(
                "Invitation has been cancelled.",
                StatusCodes.Status400BadRequest);
        }

        if (invitation.ExpiresAt < DateTime.Now)
        {
            invitation.Status = InvitationStatus.Expired;

            invitation.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            throw new ApiException(
                "Invitation has expired.",
                StatusCodes.Status400BadRequest);
        }

        return new InvitationDetailsResponse
        {
            FullName = invitation.FullName,
            Email = invitation.Email,
            ExpiresAt = invitation.ExpiresAt
        };
    }
    public async Task RegisterFromInvitationAsync(
      RegisterFromInvitationRequest request)
    {

        // 1. Find Invitation
        var invitation = await _context.Invitations
            .FirstOrDefaultAsync(x =>
                x.Token == request.Token &&
                !x.IsDeleted);

        if (invitation == null)
        {
            throw new ApiException(
                "Invitation not found.",
                StatusCodes.Status404NotFound);
        }

        // 2. Already Accepted
        if (invitation.Status == InvitationStatus.Accepted)
        {
            throw new ApiException(
                "Invitation has already been accepted.",
                StatusCodes.Status400BadRequest);
        }

        // 3. Cancelled
        if (invitation.Status == InvitationStatus.Cancelled)
        {
            throw new ApiException(
                "Invitation has been cancelled.",
                StatusCodes.Status400BadRequest);
        }

        // 4. Expired
        if (invitation.ExpiresAt < DateTime.Now)
        {
            invitation.Status = InvitationStatus.Expired;
            invitation.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            throw new ApiException(
                "Invitation has expired.",
                StatusCodes.Status400BadRequest);
        }
        using var transaction =
           await _context.Database.BeginTransactionAsync();
        try
        {
            // 5. Create User
            await _userService.CreateUserAsync(
                new CreateUserRequest
                {
                    FullName = invitation.FullName,
                    Email = invitation.Email,
                    Password = request.Password
                });

            // 6. Update Invitation
            invitation.Status = InvitationStatus.Accepted;
            invitation.AcceptedAt = DateTime.Now;
            invitation.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    public async Task<PagedResult<InvitationListItemResponse>>
    GetInvitationsAsync(
    GetInvitationsRequest request)
    {
        var query = _context.Invitations
            .Include(x => x.InvitedByUser)
            .Where(x => !x.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();

            query = query.Where(x =>
                x.FullName.ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync();

        var data = query.ToList();


        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new InvitationListItemResponse
            {
                Id = x.Id,

                FullName = x.FullName,

                Email = x.Email,

                Status = x.Status,

                CreatedAt = x.CreatedAt,

                ExpiresAt = x.ExpiresAt,

                AcceptedAt = x.AcceptedAt
            })
            .ToListAsync();

        return new PagedResult<InvitationListItemResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}