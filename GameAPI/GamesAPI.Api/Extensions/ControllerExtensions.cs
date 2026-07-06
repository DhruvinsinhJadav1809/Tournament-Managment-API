using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GamesAPI.Api.Extensions
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Extracts the user ID from the current user's claims and converts it to a Guid.
        /// Returns Guid.Empty if the user ID is not found or cannot be parsed.
        /// </summary>
        public static Guid GetUserId(this ControllerBase controller)
        {
            var userId = controller.User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            return userId != null ? Guid.Parse(userId) : Guid.Empty;
        }
    }
}
