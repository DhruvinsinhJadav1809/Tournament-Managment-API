using GamesAPI.Api.DTOs.Games;
using GamesAPI.Api.DTOs;
using GamesAPI.Api.Interfaces;
using GamesAPI.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using GamesAPI.Api.Models;

namespace GamesAPI.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService
            _notificationService;

        public NotificationController(
            INotificationService notificationService)
        {
            _notificationService =
                notificationService;
        }
        /// <summary>
        /// Get all notifications
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult>
    GetMyNotifications()
        {
            var userId = this.GetUserId();

            var notifications =
                await _notificationService
                    .GetMyNotificationsAsync(
                        userId);

            return Ok(
                new ApiResponse<List<NotificationResponse>>
                {
                    Success = true,
                    Message =
                        "Notifications fetched successfully.",
                    Data = notifications
                });
        }

        /// <summary>
        /// Get unread notification count 
        /// </summary>
        /// <returns></returns>
        [HttpGet("unread-count")]
        public async Task<IActionResult>
        GetUnreadCount()
        {
            var userId = this.GetUserId();

            var count =
                await _notificationService
                    .GetUnreadCountAsync(
                        userId);

            return Ok(
                new ApiResponse<int>
                {
                    Success = true,
                    Message =
                        "Unread count fetched successfully.",
                    Data = count
                });
        }

        /// <summary>
        /// Mark single notification as read
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>

        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult>
            MarkAsRead(
                Guid notificationId)
        {
            var userId = this.GetUserId();

            await _notificationService
                .MarkAsReadAsync(
                    notificationId,
                    userId);

            return Ok(
                new ApiResponse<bool>
                {
                    Success = true,
                    Message =
                        "Notification marked as read.",
                    Data = true
                });
        }
        /// <summary>
        /// Mark all notification as read
        /// </summary>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPut("read-all")]
        public async Task<IActionResult>
            MarkAllAsRead()
        {
            var userId = this.GetUserId();

            await _notificationService
                .MarkAllAsReadAsync(
                    userId);

            return Ok(
                new ApiResponse<bool>
                {
                    Success = true,
                    Message =
                        "All notifications marked as read.",
                    Data = true
                });
        }
    }
}