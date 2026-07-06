namespace GamesAPI.Api.Constants
{
    public static class AnnouncementPriorityConstants
    {
        public const string Low =
            "Low";

        public const string Normal =
            "Normal";

        public const string High =
            "High";

        public const string Critical =
            "Critical";

        public static readonly List<string>
            All =
            new()
            {
                Low,
                Normal,
                High,
                Critical
            };
    }
}