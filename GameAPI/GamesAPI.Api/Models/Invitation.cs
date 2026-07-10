using System.ComponentModel.DataAnnotations;
using GamesAPI.Api.Enums;
using GamesAPI.Api.Models;

namespace GamesAPI.Api.Models;

public class Invitation
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;

    public Guid Token { get; set; }

    public Guid InvitedByUserId { get; set; }

    public InvitationStatus Status { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? AcceptedAt { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public string FullName { get; set; } = string.Empty;
    // Navigation
    public virtual User InvitedByUser { get; set; } = null!;
}