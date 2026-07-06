using GamesAPI.Api.DTOs;
using GamesAPI.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GamesAPI.Api.Extensions;
using GamesAPI.Api.Constants;



namespace GamesAPI.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/announcements")]
    public class UserAnnouncementsController
    : ControllerBase
    {
        private readonly IAnnouncementService
            _announcementService;

        public UserAnnouncementsController(
            IAnnouncementService announcementService)
        {
            _announcementService =
                announcementService;
        }
        /// <summary>
        /// Get Announcements by id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult>
            GetMyAnnouncements()
        {
            var userId =
                this.GetUserId();

            var result =
                await _announcementService
                    .GetUserAnnouncementsAsync(
                        userId);
            return Ok(new ApiResponse<List<AnnouncementResponse>>
            {
                Success = true,
                Message = "Game created successfully.",
                Data = result
            });
        }
    }
}