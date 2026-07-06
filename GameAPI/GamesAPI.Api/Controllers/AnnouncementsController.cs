using GamesAPI.Api.DTOs;
using GamesAPI.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GamesAPI.Api.Extensions;
using GamesAPI.Api.Constants;



namespace GamesAPI.Api.Controllers
{

    [Authorize(Roles = RoleConstants.Admin)]
    [ApiController]
    [Route("api/admin/announcements")]
    public class AnnouncementsController
    : ControllerBase
    {
        private readonly IAnnouncementService
            _announcementService;

        public AnnouncementsController(
            IAnnouncementService
                announcementService)
        {
            _announcementService =
                announcementService;
        }
        /// <summary>
        /// Create announcement 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult>
            CreateAnnouncement(
                CreateAnnouncementRequest request)
        {
            var adminId =
                this.GetUserId();

            await _announcementService
                .CreateAnnouncementAsync(
                    adminId,
                    request);

            return Ok(
                new ApiResponse<bool>
                {
                    Success = true,
                    Message =
                        "Announcement sent successfully.",
                    Data = true
                });
        }
    }
}