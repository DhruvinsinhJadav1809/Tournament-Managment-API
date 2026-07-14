namespace GamesAPI.Api.Jobs;

public class LoggerJob
{
    public void SayHello()
    {
        Console.WriteLine(
            $"Hello Dhruvin! Current Time : {DateTime.Now}");
    }
}