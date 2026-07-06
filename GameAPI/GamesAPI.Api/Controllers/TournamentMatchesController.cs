using GamesAPI.Api.DTOs;
using GamesAPI.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GamesAPI.Api.Extensions;
using GamesAPI.Api.Constants;

namespace GamesAPI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentMatchesController : ControllerBase
    {
        private readonly ITournamentMatchesService _tournamentMatchesService;
        private readonly ILogService _logService;

        public TournamentMatchesController(
            ITournamentMatchesService tournamentMatchesService, // Fixed type here
            ILogService logService)
        {
            _tournamentMatchesService = tournamentMatchesService;
            _logService = logService;
        }

        /// <summary>
        /// Generate Matches
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("{tournamentId}/generate-matches")]
        public async Task<IActionResult> GenerateMatches(
             Guid tournamentId,
             GenerateMatchesRequest request)
        {
            // Updated to use the corrected field name
            await _tournamentMatchesService.GenerateMatchesAsync(tournamentId, request);

            // 2. Changed 'Boolean' to standard 'bool'
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Message = "Matches generated successfully.",
            });
        }
        /// <summary>
        /// Get tournament matches
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <returns></returns>
        [HttpGet("{tournamentId}/matches")]
        public async Task<IActionResult>
            GetTournamentMatches(
                Guid tournamentId)
        {
            var response =
                await _tournamentMatchesService
                    .GetTournamentMatchesAsync(
                        tournamentId);

            return Ok(new ApiResponse<TournamentMatchesResponse>
            {
                Success = true,
                Message = "Games retrieved successfully.",
                Data = response
            });
        }

        /// <summary>
        /// Update winner api
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="request"></param>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPut("{matchId}/result")]
        public async Task<IActionResult>
                  UpdateMatchResult(
                      Guid matchId,
                      UpdateMatchResultRequest request)
        {
            await _tournamentMatchesService
                .UpdateMatchResultAsync(
                    matchId,
                    request);
            return Ok(new ApiResponse<Guid>
            {
                Success = true,
                Message =
                               "Match result updated successfully.",
                Data = matchId
            });

        }
        /// <summary>
        /// Update Match schedule date from second round
        /// </summary>
        /// <param name="matchId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPut("{matchId}/schedule")]
        public async Task<IActionResult>
UpdateMatchSchedule(
    Guid matchId,
    UpdateMatchScheduleRequest request)
        {
            await _tournamentMatchesService
                .UpdateMatchScheduleAsync(
                    matchId,
                    request);

            return Ok(new ApiResponse<Guid>
            {
                Success = true,
                Message = "Match schedule updated successfully.",
                Data = matchId
            });
        }
        /// <summary>
        /// Login user upcoming match
        /// </summary>
        /// <returns></returns>
        [HttpGet("upcoming-matches")]
        public async Task<IActionResult>
    GetUpcomingMatches()
        {
            var userId =
                this.GetUserId();

            var response =
                await _tournamentMatchesService
                    .GetUpcomingMatchesAsync(
                        userId);

            return Ok(
                new ApiResponse<
                    List<UpcomingMatchResponse>>
                {
                    Success = true,
                    Message =
                        "Upcoming matches fetched successfully.",
                    Data = response
                });
        }
    }
}