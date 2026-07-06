using Microsoft.AspNetCore.Http;

namespace GamesAPI.Api.DTOs.Users
{
    public class UploadProfileImageRequest
    {
        public IFormFile File { get; set; } = null!;
    }
}