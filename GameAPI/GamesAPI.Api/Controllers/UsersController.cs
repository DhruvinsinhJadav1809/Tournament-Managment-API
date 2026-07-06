using Microsoft.AspNetCore.Mvc;
using GamesAPI.Api.DTOs;
using GamesAPI.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using GamesAPI.Api.Extensions;

namespace GamesAPI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController
        : ControllerBase
    {
        private readonly IUserService
            _userService;
        private readonly ILogService
            _logService;

        public UsersController(
            IUserService userService,
             ILogService logService)
        {
            _userService =
                userService;
            _logService =
                logService;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetUsersRequest request)
        {
            var users = await _userService.GetAllUsersAsync(request);
            return Ok(new ApiResponse<GetAllUserResponse>
            {
                Success = true,
                Message = "Users retrieved successfully.",
                Data = users
            });
        }

        /// <summary>
        /// Upload profile image for user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="file">Image file to upload</param>
        /// <returns>URL of the uploaded profile image</returns>
        [Authorize]
        [HttpPost("{id}/upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage(Guid id, IFormFile file)
        {
            try
            {
                var imageUrl = await _userService.UploadProfileImageAsync(id, file);
                await _logService.LogAsync(
                                        level: "Information",
                                    message: $"Profile image uploaded for user '{id}'.",
                                    category: "UsersController",
                                    functionName: "UploadProfileImage",
                                    userId: id,
                                    moduleType: "API",
                                    page: HttpContext.Request.Path);
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Profile image uploaded successfully.",
                    Data = imageUrl
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpGet("profile-image")]
        public async Task<IActionResult>
        /// <!----> <summary>
        /// Get profile image of the logged-in user
        ///     
        GetProfileImage()
        {
            var userId = this.GetUserId();
            var result =
                await _userService
                    .GetProfileImageAsync(
                        userId);

            return File(
                result.FileBytes,
                result.ContentType);
        }

        /// <summary>
        /// Get user by ID
        ///     </summary>
        ///     <param name="id">User ID</param>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "User not found." });
            }

            return Ok(new ApiResponse<GetUserResponse>
            {
                Success = true,
                Message = "User retrieved successfully.",
                Data = user
            });
        }

        /// <summary>
        /// Get user profile of the logged-in user
        /// </summary>
        /// <returns>User profile information</returns>
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = this.GetUserId();
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "User not found." });
            }

            return Ok(new ApiResponse<GetUserResponse>
            {
                Success = true,
                Message = "User profile retrieved successfully.",
                Data = user
            });
        }

        /// <summary>
        /// Get Full profile of user
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("my-profile")]
        public async Task<IActionResult>
            GetMyProfile()
        {
            var userId =
                this.GetUserId();

            var profile =
                await _userService
                    .GetMyProfileAsync(
                        userId);

            return Ok(
                new ApiResponse<GetMyProfileResponse>
                {
                    Success = true,
                    Message =
                        "Profile fetched successfully.",
                    Data = profile
                });
        }
    }
}