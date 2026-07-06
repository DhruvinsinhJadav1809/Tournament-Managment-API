using GamesAPI.Api.DTOs;

using GamesAPI.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace GamesAPI.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService
            _adminDashboardService;

        public AdminDashboardController(
            IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService =
                adminDashboardService;
        }
        /// <summary>
        /// Admin dash board details of summary
        /// </summary>
        /// <returns></returns>
        [HttpGet("dashboard")]
        public async Task<IActionResult>
    GetDashboard()
        {
            var response =
                await _adminDashboardService
                    .GetDashboardAsync();

            return Ok(
                new ApiResponse<AdminDashboardResponse>
                {
                    Success = true,
                    Message =
                        "Dashboard fetched successfully.",
                    Data = response
                });
        }

        /// <summary>
        /// Admin pending actions
        /// </summary>
        /// <returns></returns>
        [HttpGet("pending-actions")]
        public async Task<IActionResult>
    GetPendingActions()
        {
            var response =
                await _adminDashboardService
                    .GetPendingActionsAsync();

            return Ok(
                new ApiResponse<List<PendingActionResponse>>
                {
                    Success = true,
                    Message =
                        "Dashboard fetched successfully.",
                    Data = response
                });
        }

        /// <summary>
        /// Tournament Overview
        /// </summary>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("tournament-overview")]
        public async Task<IActionResult>
    GetTournamentOverview()
        {
            var response =
                await _adminDashboardService
                    .GetTournamentOverviewAsync();

            return Ok(
                new ApiResponse<
                    List<TournamentOverviewResponse>>
                {
                    Success = true,
                    Message =
                        "Tournament overview fetched successfully.",
                    Data = response
                });
        }
        /// <summary>
        /// Upcoming matches summary
        /// </summary>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("upcoming-matches")]
        public async Task<IActionResult>
    GetUpcomingMatches()
        {
            var response =
                await _adminDashboardService
                    .GetUpcomingMatchesAsync();

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
        /// <summary>
        /// Top Player for admin
        /// </summary>
        /// <returns></returns>
        [HttpGet("top-players")]
        public async Task<IActionResult>
    GetTopPlayers()
        {
            var response =
                await _adminDashboardService
                    .GetTopPlayersAsync();

            return Ok(
                new ApiResponse<
                    List<TopPlayerResponse>>
                {
                    Success = true,
                    Message =
                        "Top players fetched successfully.",
                    Data = response
                });
        }
        /// <summary>
        /// Recent activity
        /// </summary>
        /// <returns></returns>
        [HttpGet("recent-activities")]
        public async Task<IActionResult>
    GetRecentActivities()
        {
            var response =
                await _adminDashboardService
                    .GetRecentActivitiesAsync();

            return Ok(
                new ApiResponse<
                    List<RecentActivityResponse>>
                {
                    Success = true,
                    Message =
                        "Recent activities fetched successfully.",
                    Data = response
                });
        }
        /// <summary>
        /// Export as excel the summary of tournament
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/export")]
        public async Task<IActionResult>
    Export(Guid id)
        {
            var file =
                await _adminDashboardService
                    .ExportTournamentReportAsync(id);

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Tournament_{id}.xlsx");
        }
    }
}