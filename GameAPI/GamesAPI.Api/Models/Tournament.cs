using System.ComponentModel.DataAnnotations;

namespace GamesAPI.Api.Models
{
    public class Tournament
    {
        public Guid Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }
            = string.Empty;

        // Foreign key
        public Guid GameId { get; set; }

        [MaxLength(50)]
        public string TournamentType
        {
            get;
            set;
        } = string.Empty;

        public int MaxParticipants
        {
            get;
            set;
        }

        public DateTime? StartDate
        {
            get;
            set;
        }

        public DateTime? EndDate
        {
            get;
            set;
        }
        public DateTime? RegistrationStartDate
        {
            get;
            set;
        }

        public DateTime? RegistrationEndDate
        {
            get;
            set;
        }
        public int ParticipantsPerEntry
        {
            get;
            set;
        }
        [MaxLength(500)]
        public string? Description
        {
            get;
            set;
        }

        public bool IsActive
        {
            get;
            set;
        } = true;

        // Soft delete
        public bool IsDeleted
        {
            get;
            set;
        }

        public DateTime? DeletedAt
        {
            get;
            set;
        }

        public DateTime CreatedAt
        {
            get;
            set;
        } = DateTime.Now;
        public Guid CreatedByUserId
        {
            get;
            set;
        }
        public DateTime? UpdatedAt
        {
            get;
            set;
        }
        public Guid? updatedByUserId
        {
            get;
            set;
        }
        public Guid? WinnerId
        {
            get;
            set;
        }
        public User? Winner { get; set; }
        // Navigation property
        public Game Game { get; set; } = null!;
        public virtual ICollection<TournamentParticipant> TournamentParticipants { get; set; } = new List<TournamentParticipant>();
        public virtual ICollection<TournamentRound> TournamentRounds { get; set; } = new List<TournamentRound>();
        public virtual ICollection<TournamentMatch> TournamentMatches { get; set; } = new List<TournamentMatch>();

    }
}