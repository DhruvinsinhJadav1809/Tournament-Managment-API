using GamesAPI.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GamesAPI.Api.Controllers
{
    [ApiController]
    [Route("api/online-users")]
    [Authorize(Roles = "Admin")]
    public class OnlineUsersController : ControllerBase
    {
        private readonly IOnlineUserService _onlineUserService;

        public OnlineUsersController(
            IOnlineUserService onlineUserService)
        {
            _onlineUserService = onlineUserService;
        }
        /// <summary>
        /// Get online user 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOnlineUsers()
        {
            var result = await _onlineUserService.GetOnlineUsersAsync();
            return Ok(new ApiResponse<OnlineUsersResponse>
            {
                Success = true,
                Message = "Users retrieved successfully.",
                Data = result
            });
        }

        /// <summary>
        /// Get count of online user
        /// </summary>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("count")]
        public async Task<IActionResult> GetOnlineUserCount()
        {
            var count = await _onlineUserService.GetOnlineUserCountAsync();

            return Ok(new ApiResponse<int>
            {
                Success = true,
                Message = "Count retrieved successfully.",
                Data = count
            });
        }
    }
}