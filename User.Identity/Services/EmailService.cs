using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using User.Identity.Herlpers;
using Amazon;
using User.Identity.Helpers;

namespace User.Identity.Services;
public interface IEmailService
{
    Task Send(string to, string subject, string html, string from = null);
}

public class EmailService : IEmailService
{
    private readonly AppSettings _appSettings;

    public EmailService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    public async Task Send(string to, string subject, string html, string from = null)
    {
        AWSConfigs.AWSRegion = RegionEndpoint.APSouth1.SystemName;
        var fromAddress = new MailAddress(_appSettings.SenderEmail, _appSettings.SenderName);
        var toAddress = new MailAddress(to);
        var smtpClient = new SmtpClient
        {
            Host = _appSettings.SmtpHost,
            Port = _appSettings.SmtpPort,
            EnableSsl = true,
            Credentials = new NetworkCredential(_appSettings.SmtpUsername, _appSettings.SmtpPassword)
        };
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = html
        };
        if (!string.IsNullOrEmpty(_appSettings.CcTo))
        {
            string[] ccRecipients = _appSettings.CcTo.Split(",");
            foreach (string recipient in ccRecipients)
            {
                message.CC.Add(recipient);
            }
        }

        if (!string.IsNullOrEmpty(_appSettings.BccTo))
        {
            string[] bccRecipients = _appSettings.BccTo.Split(",");

            foreach (string recipient in bccRecipients)
            {
                message.Bcc.Add(recipient);
            }
        }

        try
        {
            await smtpClient.SendMailAsync(message);
            Console.WriteLine("Email sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
    }
}