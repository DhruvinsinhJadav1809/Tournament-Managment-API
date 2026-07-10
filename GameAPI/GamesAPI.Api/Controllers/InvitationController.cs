using GamesAPI.Api.Constants;
using GamesAPI.Api.DTOs;
using GamesAPI.Api.DTOs.Games;
using GamesAPI.Api.Extensions;
using GamesAPI.Api.Interfaces;
using GamesAPI.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesAPI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _invitationService;

        public InvitationController(
            IInvitationService invitationService
        )
        {
            _invitationService = invitationService;

        }


        /// <summary>
        /// Invite User
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.Admin)]

        [HttpPost("send")]
        public async Task<IActionResult>
                InviteUser(
                    InviteUserRequest request)
        {
            var adminId =
                this.GetUserId();

            await _invitationService
                .InviteUserAsync(
                    adminId,
                     request);

            return Ok(
                new ApiResponse<bool>
                {
                    Success = true,
                    Message =
                        "Invitation sent successfully.",
                    Data = true
                });
        }
        /// <summary>
        /// Get the user details from token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("{token:guid}")]
        public async Task<IActionResult> GetInvitation(
    Guid token)
        {
            var response = await _invitationService
                .GetInvitationAsync(token);

            return Ok(new ApiResponse<InvitationDetailsResponse>
            {
                Success = true,
                Message = "Invitation retrieved successfully.",
                Data = response
            });
        }
        /// <summary>
        /// Register user from the token and invitation
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(
    RegisterFromInvitationRequest request)
        {
            await _invitationService
                .RegisterFromInvitationAsync(request);

            return Ok(new ApiResponse<Boolean>
            {
                Success = true,
                Message = "Registration completed successfully."
            });
        }

        /// <summary>
        /// Get list of invited user
        /// </summary>
        [Authorize(Roles = RoleConstants.Admin)]

        [HttpGet]
        public async Task<IActionResult> GetInvitations(
                [FromQuery] GetInvitationsRequest request)
        {
            var response = await _invitationService
                .GetInvitationsAsync(request);

            return Ok(new ApiResponse<PagedResult<InvitationListItemResponse>>
            {
                Success = true,
                Message = "Invitations retrieved successfully.",
                Data = response
            });
        }


    }
}