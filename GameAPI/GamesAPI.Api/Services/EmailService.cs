using GamesAPI.Api.Interfaces;
using GamesAPI.Api.Models;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using GamesAPI.Api.Exceptions;

namespace GamesAPI.Api.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(
        IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendEmailAsync(
    string toEmail,
    string subject,
    string body,
    byte[]? attachment = null,
    string? attachmentName = null,
    bool isHtml = true)
    {
        var email = new MimeMessage();

        email.From.Add(
            new MailboxAddress(
                _settings.FromName,
                _settings.FromEmail));

        email.To.Add(
            MailboxAddress.Parse(toEmail));

        email.Subject = subject;
        var builder = new BodyBuilder();

        if (isHtml)
        {
            builder.HtmlBody = body;
        }
        else
        {
            builder.TextBody = body;
        }

        if (attachment != null && !string.IsNullOrWhiteSpace(attachmentName))
        {
            builder.Attachments.Add(
                attachmentName,
                attachment,
                ContentType.Parse("application/pdf"));
        }

        email.Body = builder.ToMessageBody();

        try
        {
            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _settings.UserName,
                _settings.Password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            throw new ApiException(
                $"Failed to send email. {ex.Message}",
                StatusCodes.Status500InternalServerError);
        }
    }
}