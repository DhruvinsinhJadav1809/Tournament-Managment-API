namespace GamesAPI.Api.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string FullName
        {
            get;
            set;
        } = string.Empty;

        public string Email
        {
            get;
            set;
        } = string.Empty;

        public string PasswordHash
        {
            get;
            set;
        } = string.Empty;

        public Guid RoleId
        {
            get;
            set;
        }

        public Role Role
        {
            get;
            set;
        } = null!;

        public bool IsActive
        {
            get;
            set;
        } = true;

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

        public string? ProfileImageUrl { get; set; }
        public virtual ICollection<TournamentParticipant>
        TournamentParticipants
        { get; set; }
        = new List<TournamentParticipant>();
        public virtual ICollection<TournamentMatch>
    Player1Matches
        { get; set; }
        = new List<TournamentMatch>();

        public virtual ICollection<TournamentMatch>
            Player2Matches
        { get; set; }
                = new List<TournamentMatch>();

        public virtual ICollection<TournamentMatch>
            WonMatches
        { get; set; }
                = new List<TournamentMatch>();
    }
}