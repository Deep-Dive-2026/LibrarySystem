using LibrarySystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibrarySystem.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(
        ILogger<EmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            """
            EMAIL SENT

            To: {To}
            Subject: {Subject}

            {Body}
            """,
            to,
            subject,
            body);

        return Task.CompletedTask;
    }
}