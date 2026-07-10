
public class WinnerCertificateRequest
{
    public string WinnerName { get; set; } = string.Empty;

    public string TournamentName { get; set; } = string.Empty;

    public string GameName { get; set; } = string.Empty;

    public DateTime AwardDate { get; set; }

    public string IssuedBy { get; set; } = string.Empty;

    public Guid CertificateId { get; set; }
}