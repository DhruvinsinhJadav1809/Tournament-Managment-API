using GamesAPI.Api.Models;

public static class TournamentStatusHelper
{
    public static string GetStatus(
        Tournament tournament)
    {
        var currentDate =
            DateTime.Now;

        if (currentDate <
            tournament.RegistrationStartDate)
        {
            return "Upcoming";
        }

        if (currentDate <=
            tournament.RegistrationEndDate)
        {
            return "RegistrationOpen";
        }

        if (currentDate <
            tournament.StartDate)
        {
            return "RegistrationClosed";
        }

        if (currentDate <=
            tournament.EndDate)
        {
            return "Ongoing";
        }

        return "Completed";
    }
}