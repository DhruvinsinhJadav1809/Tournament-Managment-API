using Microsoft.AspNetCore.Mvc;
using GamesAPI.Api.DTOs.User;
using GamesAPI.Api.DTOs;
using GamesAPI.Api.Interfaces;
using GamesAPI.Api.Exceptions;

namespace GamesAPI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController
        : ControllerBase
    {
        private readonly IUserService
            _userService;
        private readonly ILogService _logService;
        public AuthController(
            IUserService userService,
            ILogService logService)
        {
            _userService =
                userService;
            _logService =
                logService;
        }

        /// <summary>
        /// Register user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult>
            CreateUser(
            CreateUserRequest request)
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

            var user =
                await _userService
                    .CreateUserAsync(
                        request);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "User created successfully.",
                Data = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.Role
                }
            });
        }
        /// <summary>
        /// Login user
        /// </summary>  
        [HttpPost("login")]
        public async Task<IActionResult>
        Login(
        LoginRequest request)
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


            var response =
                await _userService
                    .LoginAsync(
                        request);
            await _logService.LogAsync(
                                                    level: "Information",
                                                    message: $"User with email '{request.Email}' logged in successfully.",
                                                    category: "AuthController",
                                                    functionName: "Login",
                                                    userId: response.Id,
                                                    moduleType: "API",
                                                    page: HttpContext.Request.Path);

            return Ok(new ApiResponse<LoginResponse>
            {
                Success = true,
                Message = "Login successful.",
                Data = response
            });
        }
    }
}