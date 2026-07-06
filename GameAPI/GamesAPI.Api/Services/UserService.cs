
using Microsoft.EntityFrameworkCore;
using GamesAPI.Api.Data;
using GamesAPI.Api.DTOs.User;
using GamesAPI.Api.Interfaces;
using GamesAPI.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using GamesAPI.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using GamesAPI.Api.Constants;


namespace GamesAPI.Api.Services
{
    public class UserService
        : IUserService
    {
        private readonly AppDbContext
     _context;

        private readonly IConfiguration
            _configuration;

        public UserService(
            AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration =
                configuration;
        }

        private string
    GenerateJwtToken(
    User user)
        {
            var jwtSettings =
                _configuration
                    .GetSection("Jwt");

            // Claims
            var claims =
                new List<Claim>
                {
            new Claim(
                ClaimTypes
                    .NameIdentifier,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.Email,
                user.Email),

            new Claim(
                ClaimTypes.Name,
                user.FullName),

            new Claim(
                ClaimTypes.Role,
                user.Role.Name)
                };

            // Secret key
            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8
                        .GetBytes(
                            jwtSettings["Key"]!));

            // Signature
            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms
                        .HmacSha256);

            // Expiry
            var expires =
                DateTime.Now
                    .AddMinutes(
                        Convert.ToDouble(
                            jwtSettings[
                                "ExpireMinutes"]));

            // Token
            var token =
                new JwtSecurityToken(
                    issuer:
                        jwtSettings[
                            "Issuer"],

                    audience:
                        jwtSettings[
                            "Audience"],

                    claims:
                        claims,

                    expires:
                        expires,

                    signingCredentials:
                        credentials);

            return new
                JwtSecurityTokenHandler()
                    .WriteToken(
                        token);
        }

        public async Task<User>
            CreateUserAsync(
            CreateUserRequest request)
        {
            // Check email exists
            var emailExists =
                await _context.Users
                    .AnyAsync(x =>
                        !x.IsDeleted
                        && x.Email.ToLower()
                        ==
                        request.Email
                            .Trim()
                            .ToLower());

            if (emailExists)
            {
                throw new ApiException(
                    "Email already exists.",
                    StatusCodes.Status400BadRequest);
            }

            // Hash password
            var hashedPassword =
                BCrypt.Net.BCrypt
                    .HashPassword(
                        request.Password);

            Guid roleId;

            if (request.RoleId.HasValue)
            {
                roleId = request.RoleId.Value;
            }
            else
            {
                roleId = await _context.Roles
                        .Where(x =>
                            x.Name == RoleConstants.User &&
                            x.IsActive)
                        .Select(x => x.Id)
                        .FirstOrDefaultAsync();

                if (roleId == Guid.Empty)
                {
                    throw new ApiException(
                        "Default user role not found.",
                        StatusCodes.Status500InternalServerError);
                }
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName =
                    request.FullName.Trim(),

                Email =
                    request.Email
                        .Trim()
                        .ToLower(),

                PasswordHash =
                    hashedPassword,

                RoleId = roleId,

                IsActive = true,

                CreatedAt =
                    DateTime.Now
            };

            _context.Users.Add(
                user);

            await _context
                .SaveChangesAsync();

            return user;
        }

        public async Task<GetAllUserResponse>
         GetAllUsersAsync(
           GetUsersRequest request)
        {
            var query =
                _context.Users
                    .Include(x => x.Role)
                    .Where(x => !x.IsDeleted)
                    .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(
                    request.Search))
            {
                var search =
                    request.Search.Trim();

                query = query.Where(x =>
                    x.FullName.Contains(search)
                    || x.Email.Contains(search));
            }

            var totalRecords =
                await query.CountAsync();

            var users =
                await query
                    .OrderBy(x => x.FullName)
                    .Skip(
                        (request.PageNumber - 1)
                        * request.PageSize)
                    .Take(
                        request.PageSize)
                    .Select(x =>
                        new GetUsersResponse
                        {
                            Id = x.Id,
                            FullName = x.FullName,
                            Email = x.Email,
                            Role = x.Role.Name,
                            IsActive = x.IsActive,
                            CreatedAt = x.CreatedAt
                        })
                    .ToListAsync();

            return new GetAllUserResponse
            {
                Page =
                    request.PageNumber,

                PageSize =
                    request.PageSize,

                TotalCount =
                    totalRecords,

                Data =
                    users
            };
        }

        public async Task<LoginResponse>
    LoginAsync(
    LoginRequest request)
        {
            var user =
                await _context.Users
                    .Include(x => x.Role)
                    .FirstOrDefaultAsync(x =>
                        !x.IsDeleted
                        &&
                        x.Email.ToLower()
                        ==
                        request.Email
                            .Trim()
                            .ToLower());

            // Don't reveal email exists
            if (user == null)
            {
                throw new ApiException(
                    "Invalid email or password.",
                    StatusCodes.Status401Unauthorized);
            }

            // Active check
            if (!user.IsActive)
            {
                throw new ApiException(
                    "Your account is inactive. Please contact support.",
                    StatusCodes.Status401Unauthorized);
            }

            // Verify password
            var isPasswordCorrect =
                BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.PasswordHash);

            if (!isPasswordCorrect)
            {
                throw new ApiException(
                    "Invalid email or password.",
                    StatusCodes.Status401Unauthorized);
            }

            return new LoginResponse
            {
                Id = user.Id,
                FullName =
                    user.FullName,
                Email =
                    user.Email,
                Role =
                    user.Role.Name,


                Token =
                    GenerateJwtToken(
                        user)
            };
        }

        public async Task<string>
    UploadProfileImageAsync(
        Guid userId,
        IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new BadRequestException(
                    "Please select a file.");
            }

            var allowedExtensions =
                new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png"
                };

            var extension =
                Path.GetExtension(
                    file.FileName)
                    .ToLower();

            if (!allowedExtensions.Contains(
                    extension))
            {
                throw new BadRequestException(
                    "Only JPG, JPEG and PNG files are allowed.");
            }

            var allowedContentTypes =
                new[]
                {
                    "image/jpeg",
                    "image/png"
                };

            if (!allowedContentTypes.Contains(
                    file.ContentType))
            {
                throw new BadRequestException(
                    "Invalid image file.");
            }

            const long maxFileSize =
                5 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                throw new BadRequestException(
                    "File size cannot exceed 5 MB.");
            }

            var user =
                await _context.Users
                    .FirstOrDefaultAsync(x =>
                     x.Id == userId);

            if (user == null)
            {
                throw new NotFoundException(
                    "User not found.");
            }
            var uploadsFolder =
                _configuration[
                    "FileStorage:ProfilePhotoPath"];
            if (!string.IsNullOrWhiteSpace(
                    user.ProfileImageUrl))
            {
                var oldFilePath =
                    Path.Combine(
                        uploadsFolder!,
                        user.ProfileImageUrl);
                try
                {
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }
                catch
                {
                }
            }
            var fileName =
                    $"{Guid.NewGuid()}{extension}";


            if (!Directory.Exists(
            uploadsFolder))
            {
                Directory.CreateDirectory(
                    uploadsFolder);
            }
            var filePath =
    Path.Combine(
        uploadsFolder!,
        fileName);

            using var stream =
                new FileStream(
                    filePath,
                    FileMode.Create);

            await file.CopyToAsync(
                stream);
            user.ProfileImageUrl = fileName;

            await _context.SaveChangesAsync();
            return fileName;
        }


        public async Task<
         (byte[] FileBytes, string ContentType)>
         GetProfileImageAsync(
             Guid userId)
        {
            var user =
                await _context.Users
                    .FirstOrDefaultAsync(x =>
                        x.Id == userId);

            if (user == null)
            {
                throw new NotFoundException(
                    "User not found.");
            }

            if (string.IsNullOrWhiteSpace(
                    user.ProfileImageUrl))
            {
                throw new NotFoundException(
                    "Profile image not found.");
            }

            var uploadsFolder =
                _configuration[
                    "FileStorage:ProfilePhotoPath"];

            var filePath =
                Path.Combine(
                    uploadsFolder!,
                    user.ProfileImageUrl);

            if (!File.Exists(
                    filePath))
            {
                throw new NotFoundException(
                    "Profile image not found.");
            }

            var fileBytes =
                await File.ReadAllBytesAsync(
                    filePath);

            var extension =
                Path.GetExtension(
                    user.ProfileImageUrl)
                    .ToLower();

            var contentType =
                extension switch
                {
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };

            return (
                fileBytes,
                contentType);
        }

        public async Task<GetUserResponse> GetUserByIdAsync(
            Guid userId)
        {
            var user =
                await _context.Users
                 .Include(x => x.Role)
                    .FirstOrDefaultAsync(x =>
                        x.Id == userId);

            if (user == null)
            {
                throw new NotFoundException(
                    "User not found.");
            }

            return new GetUserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.Name,
                HasProfileImage = !string.IsNullOrWhiteSpace(user.ProfileImageUrl)
            };
        }

        public async Task<GetMyProfileResponse>
        GetMyProfileAsync(
            Guid userId)
        {
            var user =
                await _context.Users
                    .Include(x => x.Role)
                    .FirstOrDefaultAsync(x =>
                        x.Id == userId &&
                        !x.IsDeleted);

            if (user == null)
            {
                throw new NotFoundException(
                    "User not found.");
            }

            var matchesPlayed =
                await _context.TournamentMatches
                    .CountAsync(x =>
                        x.Status == "Completed" &&
                        (
                            x.Player1Id == userId ||
                            x.Player2Id == userId
                        ));

            var wins =
                await _context.TournamentMatches
                    .CountAsync(x =>
                        x.WinnerId == userId);

            var losses =
                matchesPlayed - wins;

            decimal winRate = 0;

            if (matchesPlayed > 0)
            {
                winRate =
                    Math.Round(
                        (decimal)wins * 100 /
                        matchesPlayed,
                        2);
            }

            var championships =
                await _context.Tournaments
                    .CountAsync(x =>
                        x.WinnerId == userId);

            var currentDate = DateTime.Now;

            var activeTournaments =
                await _context.TournamentParticipants
                    .CountAsync(x =>
                        x.UserId == userId &&
                        (
                            currentDate <
                                x.Tournament.RegistrationStartDate ||

                            currentDate <=
                                x.Tournament.RegistrationEndDate ||

                            currentDate <
                                x.Tournament.StartDate ||

                            currentDate <=
                                x.Tournament.EndDate
                        ));

            var completedTournaments =
            await _context.TournamentParticipants
            .CountAsync(x =>
             x.UserId == userId &&
             currentDate >
                 x.Tournament.EndDate);

            var recentMatches =
                await _context.TournamentMatches
                    .Include(x => x.Player1)
                    .Include(x => x.Player2)
                    .Include(x => x.Tournament)
                    .Where(x =>
                        x.Status == "Completed" &&
                        (
                            x.Player1Id == userId ||
                            x.Player2Id == userId
                        ))
                    .OrderByDescending(x =>
                        x.StartTime)
                    .Take(5)
                    .ToListAsync();

            var recentMatchResponses =
                recentMatches
                    .Select(match =>
                    {
                        bool isPlayer1 =
                            match.Player1Id == userId;

                        var opponentName =
                            isPlayer1
                                ? match.Player2?.FullName ?? "Unknown Player"
                                : match.Player1?.FullName ?? "Unknown Player";

                        var result =
                            match.WinnerId == userId
                                ? "Won"
                                : "Lost";

                        var score =
                            isPlayer1
                                ? $"{match.Player1Score} - {match.Player2Score}"
                                : $"{match.Player2Score} - {match.Player1Score}";

                        return new RecentMatchResponse
                        {
                            MatchId = match.Id,
                            OpponentName = opponentName,
                            TournamentName =
                                match.Tournament.Name,
                            Result = result,
                            Score = score,
                            MatchDate =
                                match.StartTime ??
                                DateTime.MinValue
                        };
                    })
                    .ToList();

            return new GetMyProfileResponse
            {
                FullName = user.FullName,

                Email = user.Email,

                Role = user.Role.Name,

                MatchesPlayed = matchesPlayed,

                Wins = wins,

                Losses = losses,

                WinRate = winRate,

                Championships =
                    championships,

                ActiveTournaments =
                    activeTournaments,

                CompletedTournaments =
                    completedTournaments,

                RecentMatches =
                    recentMatchResponses,
                HasProfileImage = !string.IsNullOrWhiteSpace(user.ProfileImageUrl)
            };
        }
    }


}