public class TopPlayerResponse
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = "";

    public int Wins { get; set; }

    public int Championships { get; set; }

    public decimal WinRate { get; set; }
}