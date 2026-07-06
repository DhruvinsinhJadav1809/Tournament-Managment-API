namespace GamesAPI.Api.DTOs.Tournaments
{
    public class TournamentResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
            = string.Empty;

        public Guid GameId { get; set; }

        public string GameName { get; set; }
            = string.Empty;

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


        public string StatusName
        {
            get;
            set;
        } = null!;
        public bool IsActive
        {
            get;
            set;
        }
        public int CurrentParticipants
        {
            get; set;
        }
        public bool IsGeneratedMatches
        {
            get;
            set;
        }
    }
}