using GamesAPI.Api.DTOs.Tournaments;
using GamesAPI.Api.DTOs;
using GamesAPI.Api.Interfaces;
using GamesAPI.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GamesAPI.Api.Constants;

namespace GamesAPI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentsController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly ILogService _logService;

        public TournamentsController(
            ITournamentService tournamentService,
            ILogService logService)
        {
            _tournamentService = tournamentService;
            _logService = logService;
        }

        /// <summary>
        /// Get all tournaments
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTournaments(
            [FromQuery] GetTournamentsRequest request)
        {
            var result =
                await _tournamentService
                    .GetTournamentsAsync(request);
            return Ok(new ApiResponse<GetTournamentsResponse>
            {
                Success = true,
                Message = "Tournaments retrieved successfully.",
                Data = result
            });
        }


        /// <summary>
        /// Create a new tournament
        /// </summary>
        [Authorize]
        [HttpPost]
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost]
        public async Task<IActionResult>
    CreateTournament(
    CreateTournamentRequest request)
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

            var tournament =
                await _tournamentService
                    .CreateTournamentAsync(
                        request,
                        userId);

            await _logService.LogAsync(
                                        level: "Information",
                                        message: $"Tournament '{tournament.Name}' created successfully.",
                                        category: "TournamentsController",
                                        functionName: "CreateTournament",
                                        userId: userId,
                                        moduleType: "API",
                                        page: HttpContext.Request.Path);

            return Ok(new ApiResponse<Models.Tournament>
            {
                Success = true,
                Message = "Tournament created successfully.",
                Data = tournament
            });
        }


        /// <summary>
        /// Get tournament by ID
        ///     </summary>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTournamentById(
            Guid id)
        {
            var tournament =
                await _tournamentService
                    .GetTournamentByIdAsync(id);

            if (tournament == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Tournament not found."
                });
            }

            return Ok(new ApiResponse<TournamentResponse>
            {
                Success = true,
                Message = "Tournament retrieved successfully.",
                Data = tournament
            });
        }


        /// <summary>
        /// Delete a tournament
        ///    </summary>
        /// <param name="id">Tournament ID</param>

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournament(
            Guid id)
        {
            var userId = this.GetUserId();

            var success =
                await _tournamentService
                    .DeleteTournamentAsync(
                        id,
                        userId);

            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Tournament not found or you do not have permission to delete it."
                });
            }

            await _logService.LogAsync(
                                        level: "Information",
                                        message: $"Tournament with ID '{id}' deleted successfully.",
                                        category: "TournamentsController",
                                        functionName: "DeleteTournament",
                                        userId: userId,
                                        moduleType: "API",
                                        page: HttpContext.Request.Path);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Tournament deleted successfully."
            });
        }


        /// <summary>
        /// Update a tournament
        /// </summary>
        /// <param name="id">Tournament ID</param>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTournament(
            Guid id,
            UpdateTournamentRequest request)
        {
            var userId = this.GetUserId();

            var tournament =
                await _tournamentService
                    .UpdateTournamentAsync(
                        id,
                        request,
                        userId);

            if (tournament == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Tournament not found or you do not have permission to update it."
                });
            }

            await _logService.LogAsync(
                                        level: "Information",
                                        message: $"Tournament with ID '{id}' updated successfully.",
                                        category: "TournamentsController",
                                        functionName: "UpdateTournament",
                                        userId: userId,
                                        moduleType: "API",
                                        page: HttpContext.Request.Path);

            return Ok(new ApiResponse<Models.Tournament>
            {
                Success = true,
                Message = "Tournament updated successfully.",
                Data = tournament
            });
        }


        /// <summary>
        /// Join Tournament 
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <returns></returns>
        [HttpPost("{tournamentId}/join")]
        [Authorize]
        public async Task<IActionResult>
    JoinTournament(
        Guid tournamentId)
        {
            var userId =
               this.GetUserId();

            await _tournamentService
                .JoinTournamentAsync(
                    tournamentId,
                    userId);
            return Ok(new ApiResponse<Models.Tournament>
            {
                Success = true,
                Message = "Tournament joined successfully.",
                Data = null
            });

        }


        /// <summary>
        /// For user Dashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet("dashboard")]
        [Authorize]
        public async Task<IActionResult>
    GetDashboardTournaments()
        {
            var userId = this.GetUserId();
            var response =
                await _tournamentService
                    .GetDashboardTournamentsAsync(userId);
            return Ok(new ApiResponse<List<TournamentDashboardResponse>>
            {
                Success = true,
                Message = "Tournament joined successfully.",
                Data = response
            });

        }


        /// <summary>
        /// Tournament Participants for the admin
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <returns></returns>
        [HttpGet("{tournamentId}/participants")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult>
    GetTournamentParticipants(
        Guid tournamentId)
        {
            var response =
                await _tournamentService
                    .GetTournamentParticipantsAsync(
                        tournamentId);
            return Ok(new ApiResponse<TournamentParticipantsResponse>
            {
                Success = true,
                Message = "Tournament joined successfully.",
                Data = response
            });
        }

        /// <summary>
        /// Get Login user Tournament
        /// </summary>
        /// <returns></returns>
        [HttpGet("my-tournaments")]
        public async Task<IActionResult>
            GetMyTournaments()
        {
            var userId = this.GetUserId();

            var response =
                await _tournamentService
                    .GetMyTournamentsAsync(
                        userId);
            return Ok(new ApiResponse<GetMyTournamentsResponse>
            {
                Success = true,
                Message = "Your Tournament Details",
                Data = response
            });
        }
    }
}