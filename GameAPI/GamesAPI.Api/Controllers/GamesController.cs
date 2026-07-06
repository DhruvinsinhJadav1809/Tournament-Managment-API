using GamesAPI.Api.DTOs.Games;
using GamesAPI.Api.DTOs;
using GamesAPI.Api.Interfaces;
using GamesAPI.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GamesAPI.Api.Models;
using GamesAPI.Api.Constants;

namespace GamesAPI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ILogService _logService;

        public GamesController(
            IGameService gameService,
            ILogService logService)
        {
            _gameService = gameService;
            _logService = logService;
        }
        [Authorize]
        /// <summary>
        /// Get all games
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetGames(
                    [FromQuery] GetGamesRequest request)
        {
            var result =
                await _gameService
                    .GetGamesAsync(request);

            return Ok(new ApiResponse<GetGamesResponse>
            {
                Success = true,
                Message = "Games retrieved successfully.",
                Data = result
            });
        }

        /// <summary>
        /// Get game by id
        /// </summary>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult>
             GetGame(Guid id)
        {
            var game =
                await _gameService
                    .GetGameByIdAsync(id);

            if (game == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Game not found."
                });
            }

            return Ok(new ApiResponse<Models.Game>
            {
                Success = true,
                Message = "Game retrieved successfully.",
                Data = game
            });
        }

        /// <summary>
        /// Create new game
        /// </summary>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost]
        public async Task<IActionResult>
             CreateGame(
             CreateGameRequest request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = ModelState.Values
                        .Where(v => v!.Errors.Count > 0)
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            var userId = this.GetUserId();
            var game =
                await _gameService
                    .CreateGameAsync(
                        request, userId);
            await _logService.LogAsync(
                                        level: "Information",
                                        message: $"Game '{game.Name}' created successfully.",
                                        category: "GamesController",
                                        functionName: "CreateGame",
                                        userId: userId,
                                        moduleType: "API",
                                        page: HttpContext.Request.Path);
            return Ok(new ApiResponse<Models.Game>
            {
                Success = true,
                Message = "Game created successfully.",
                Data = game
            });
        }

        /// <summary>
        /// Delete game by id
        /// </summary>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult>
             DeleteGame(Guid id)
        {
            var userId = this.GetUserId();
            var deleted =
                await _gameService
                    .DeleteGameAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Game not found."
                });
            }
            await _logService.LogAsync(
                                        level: "Information",
                                        message: $"Game with ID '{id}' deleted successfully.",
                                        category: "GamesController",
                                        functionName: "DeleteGame",
                                        userId: userId,
                                        moduleType: "API",
                                        page: HttpContext.Request.Path);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Game deleted successfully."
            });
        }

        /// <summary>
        /// Update game
        /// </summary>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult>
             UpdateGame(
             Guid id,
             UpdateGameRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = ModelState.Values
                        .Where(v => v!.Errors.Count > 0)
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }
            var userId = this.GetUserId();
            var game =
                await _gameService
                    .UpdateGameAsync(
                        id,
                        request, userId);

            if (game == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Game not found."
                });
            }
            await _logService.LogAsync(
                                                    level: "Information",
                                                    message: $"Game '{game.Name}' updated successfully.",
                                                    category: "GamesController",
                                                    functionName: "UpdateGame",
                                                    userId: userId,
                                                    moduleType: "API",
                                                    page: HttpContext.Request.Path);
            return Ok(new ApiResponse<Models.Game>
            {
                Success = true,
                Message = "Game updated successfully.",
                Data = game
            });
        }

        [Authorize]
        /// <summary>
        /// Get all games with out pagination
        /// </summary>
        [HttpGet("All")]
        public async Task<IActionResult> GetAllGames()
        {
            var result =
                await _gameService
                    .GetAllGames();

            return Ok(new ApiResponse<List<Game>>
            {
                Success = true,
                Message = "Games retrieved successfully.",
                Data = result
            });
        }

    }
}