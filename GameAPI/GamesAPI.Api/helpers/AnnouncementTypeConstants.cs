namespace GamesAPI.Api.Constants
{
    public static class AnnouncementTypeConstants
    {
        public const string Information =
            "Information";

        public const string Tournament =
            "Tournament";

        public const string Match =
            "Match";

        public const string Warning =
            "Warning";

        public const string Urgent =
            "Urgent";

        public static readonly List<string>
            All =
            new()
            {
                Information,
                Tournament,
                Match,
                Warning,
                Urgent
            };
    }
}