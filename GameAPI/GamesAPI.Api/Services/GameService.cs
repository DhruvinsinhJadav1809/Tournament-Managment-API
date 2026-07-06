using GamesAPI.Api.Data;
using GamesAPI.Api.DTOs.Games;
using GamesAPI.Api.Exceptions;
using GamesAPI.Api.Interfaces;
using GamesAPI.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GamesAPI.Api.Services
{
    public class GameService : IGameService
    {
        private readonly AppDbContext _context;

        public GameService(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetGamesResponse>
            GetGamesAsync(
            GetGamesRequest request)
        {
            var query = _context.Games
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

            // Participants filter
            if (request
                .ParticipantsPerMatch
                .HasValue)
            {
                query = query.Where(x =>
                    x.ParticipantsPerMatch ==
                    request
                    .ParticipantsPerMatch
                    .Value);
            }

            // Total count
            var totalCount =
                await query.CountAsync();

            // Pagination
            var games = await query
                .Skip(
                    (request.Page - 1)
                    * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new GetGamesResponse
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = games
            };
        }

        public async Task<Game?>
            GetGameByIdAsync(Guid id)
        {
            return await _context.Games
                .FirstOrDefaultAsync(
                    x => x.Id == id && !x.IsDeleted);
        }

        public async Task<Game> CreateGameAsync(
     CreateGameRequest request,
     Guid userId)
        {

            var gameExists =
            await _context.Games
                .AnyAsync(x =>
                    !x.IsDeleted &&
                    x.Name.ToLower() ==
                    request.Name
                        .Trim()
                        .ToLower());

            if (gameExists)
            {
                throw new ApiException(
                    "Game name already exists.",
                    StatusCodes.Status400BadRequest);
            }

            var game = new Game
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                ParticipantsPerMatch =
                    request.ParticipantsPerMatch!.Value,
                IsActive = request.IsActive,
                CreatedAt = DateTime.Now,
                CreatedByUserId = userId
            };

            _context.Games.Add(game);

            await _context.SaveChangesAsync();

            return game;
        }
        public async Task<Game?>
            UpdateGameAsync(
            Guid id,
            UpdateGameRequest request,
            Guid userId)
        {
            var game =
                await _context.Games
                    .FirstOrDefaultAsync(
                        x => x.Id == id && !x.IsDeleted);

            if (game == null)
            {
                return null;
            }

            game.Name =
                request.Name.Trim();

            game.ParticipantsPerMatch =
                request
                .ParticipantsPerMatch!
                .Value;

            game.IsActive =
                request.IsActive;

            game.UpdatedByUserId = userId;
            game.UpdatedAt = DateTime.Now;
            await _context
                .SaveChangesAsync();

            return game;
        }

        public async Task<bool>
      DeleteGameAsync(Guid id, Guid userId)
        {
            var game =
                await _context.Games
                    .FirstOrDefaultAsync(
                        x => x.Id == id
                        && !x.IsDeleted);

            if (game == null)
            {
                return false;
            }

            game.IsDeleted = true;
            game.IsActive = false;
            game.DeletedAt =
                DateTime.Now;
            game.UpdatedByUserId = userId;
            game.UpdatedAt = DateTime.Now;
            await _context
                .SaveChangesAsync();

            return true;
        }
        public async Task<List<Game>>
                  GetAllGames()
        {

            return await _context.Games
                            .Where(x => x.IsActive)
                            .ToListAsync();


        }


    }
}