namespace GamesAPI.Api.Constants
{
    public static class AnnouncementTargetTypeConstants
    {
        public const string AllUsers =
            "AllUsers";

        public const string Tournament =
            "Tournament";

        public const string Match =
            "Match";

        public const string User =
            "User";

        public static readonly List<string>
            All =
            new()
            {
                AllUsers,
                Tournament,
                Match,
                User
            };
    }
}