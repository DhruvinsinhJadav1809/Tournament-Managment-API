using GamesAPI.Api.DTOs.User;
using GamesAPI.Api.Models;

namespace GamesAPI.Api.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(
            CreateUserRequest request);
        Task<GetAllUserResponse> GetAllUsersAsync(GetUsersRequest request);

        Task<LoginResponse> LoginAsync(
           LoginRequest request);
        Task<string> UploadProfileImageAsync(
            Guid userId,
            IFormFile file);
        Task<(byte[] FileBytes, string ContentType)>
            GetProfileImageAsync(
                Guid userId);
        Task<GetUserResponse> GetUserByIdAsync(
            Guid userId);
        Task<GetMyProfileResponse> GetMyProfileAsync(Guid userId);
    }
}