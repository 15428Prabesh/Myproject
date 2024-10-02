using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using Application;

namespace SkyLearn.Portal.Api.Services
{
    public interface IEmailSendService
    {
        Task Send(string to, string subject, string html, string from = null);
    }

    public class EmailSendService : IEmailSendService
    {
        private readonly EmailSetting _emailSetting;
        public EmailSendService(IOptions<EmailSetting> emailSetting)
        {
            _emailSetting= emailSetting.Value;
        }

        public async Task Send(string to, string subject, string html, string from = null)
        {           
            var fromAddress = new MailAddress(_emailSetting.SenderEmail, _emailSetting.SenderName);
            var toAddress = new MailAddress(to);
            var smtpClient = new SmtpClient
            {
                Host = _emailSetting.SmtpHost,
                Port = _emailSetting.SmtpPort,
                EnableSsl = true,
                Credentials = new NetworkCredential(_emailSetting.SmtpUsername, _emailSetting.SmtpPassword)
            };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = html
            };
            if (!string.IsNullOrEmpty(_emailSetting.CcTo))
            {
                string[] ccRecipients = _emailSetting.CcTo.Split(",");
                foreach (string recipient in ccRecipients)
                {
                    message.CC.Add(recipient);
                }
            }

            if (!string.IsNullOrEmpty(_emailSetting.BccTo))
            {
                string[] bccRecipients = _emailSetting.BccTo.Split(",");

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
}
