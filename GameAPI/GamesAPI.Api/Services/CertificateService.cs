

using GamesAPI.Api.Exceptions;
using GamesAPI.Api.Interfaces;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using GamesAPI.Api.Data;
using Microsoft.EntityFrameworkCore;



namespace GamesAPI.Api.Services;

public class CertificateService : ICertificateService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly IEmailService _emailService;

    public CertificateService(
        IWebHostEnvironment environment,
       AppDbContext context,
       IEmailService emailService)
    {
        _environment = environment;
        _context = context;
        _emailService = emailService;
    }
    private static bool _browserDownloaded = false;

    private static async Task EnsureBrowserDownloadedAsync()
    {
        if (_browserDownloaded)
            return;

        var browserFetcher = new BrowserFetcher();

        await browserFetcher.DownloadAsync();

        _browserDownloaded = true;
    }
    public async Task<byte[]> GenerateWinnerCertificateAsync(
    WinnerCertificateRequest request)
    {
        var templatePath = Path.Combine(
            _environment.ContentRootPath,
            "Templates",

            "WinnerCertificate.html");

        var html = await File.ReadAllTextAsync(templatePath);

        html = html.Replace(
            "{{WinnerName}}",
            request.WinnerName);

        html = html.Replace(
            "{{TournamentName}}",
            request.TournamentName);

        html = html.Replace(
            "{{GameName}}",
            request.GameName);

        html = html.Replace(
            "{{AwardDate}}",
            request.AwardDate.ToString("dd MMM yyyy"));

        html = html.Replace(
            "{{IssuedBy}}",
            request.IssuedBy);

        html = html.Replace(
            "{{CertificateId}}",
            request.CertificateId.ToString());

        // PDF Generation comes next

        await EnsureBrowserDownloadedAsync();

        await using var browser =
            await Puppeteer.LaunchAsync(
                new LaunchOptions
                {
                    Headless = true
                });

        await using var page =
            await browser.NewPageAsync();

        await page.SetContentAsync(html);

        var pdf = await page.PdfDataAsync(
            new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true
            });

        return pdf;

    }

    public async Task SendWinnerCertificateAsync(
    Guid tournamentId)
    {
        var tournament = await _context.Tournaments
            .Include(x => x.Game)
            .Include(x => x.Winner)
            .FirstOrDefaultAsync(x =>
                x.Id == tournamentId &&
                !x.IsDeleted);

        if (tournament == null)
        {
            throw new ApiException(
                "Tournament not found.",
                StatusCodes.Status404NotFound);
        }

        if (tournament.Winner == null)
        {
            throw new ApiException(
                "Winner not found.",
                StatusCodes.Status400BadRequest);
        }
        var createdBy = await _context.Users
                        .Select(x => x.FullName)
                        .FirstOrDefaultAsync();
        var request = new WinnerCertificateRequest
        {
            WinnerName = tournament.Winner.FullName,

            TournamentName = tournament.Name,

            GameName = tournament.Game.Name,

            AwardDate = DateTime.Now,

            IssuedBy = createdBy ?? "Admin",

            CertificateId = tournament.Id
        };

        var certificate =
            await GenerateWinnerCertificateAsync(request);
        var templatePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            EmailTemplates.Winner);

        var html =
            await File.ReadAllTextAsync(templatePath);

        html = html.Replace(
    "{{WinnerName}}",
    request.WinnerName);

        html = html.Replace(
            "{{TournamentName}}",
            request.TournamentName);

        html = html.Replace(
            "{{GameName}}",
            request.GameName);

        html = html.Replace(
            "{{AwardDate}}",
            request.AwardDate.ToString("dd MMM yyyy"));

        html = html.Replace(
            "{{IssuedBy}}",
            request.IssuedBy);
        await _emailService.SendEmailAsync(
            tournament.Winner.Email,
            $"🏆 Congratulations! You won {tournament.Name}",
            html,
            certificate,
            $"{tournament.Name}-Winner-Certificate.pdf");
    }
}