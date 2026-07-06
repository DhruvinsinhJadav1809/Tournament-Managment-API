using GamesAPI.Api.DTOs.User;
using GamesAPI.Api.Models;

namespace GamesAPI.Api.Interfaces
{
    public interface IAuthService
    {
        Task<User> CreateUserAsync(
            CreateUserRequest request);
        Task<LoginResponse> LoginAsync(
            LoginRequest request);
    }
}