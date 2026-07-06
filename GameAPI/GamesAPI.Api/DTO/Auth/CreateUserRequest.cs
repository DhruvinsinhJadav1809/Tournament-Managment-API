using System.ComponentModel.DataAnnotations;

namespace GamesAPI.Api.DTOs.User
{
    public class CreateUserRequest
    {
        [Required(
            ErrorMessage =
            "Full name is required.")]
        public string FullName
        {
            get;
            set;
        } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [RegularExpression(
            @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",
            ErrorMessage = "Please enter a valid email address."
        )]
        public string Email { get; set; } = string.Empty;

        [Required(
            ErrorMessage =
            "Password is required.")]
        [MinLength(
            8,
            ErrorMessage =
            "Password must be at least 8 characters.")]
        public string Password
        {
            get;
            set;
        } = string.Empty;

        public Guid? RoleId { get; set; }
    }
}