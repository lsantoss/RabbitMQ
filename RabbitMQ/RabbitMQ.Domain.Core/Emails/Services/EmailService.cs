using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Emails.Interfaces.Services;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Core.Emails.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService(SmtpSettings smtpSettings)
        {
            _smtpClient = new SmtpClient
            {
                Host = smtpSettings.Host,
                Port = smtpSettings.Port,
                UseDefaultCredentials = smtpSettings.UseDefaultCredentials,
                Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password),
                EnableSsl = smtpSettings.EnableSsl
            };
        }

        public async Task SendEmailAsync(string content, string subject, string from, List<string> recipients, List<string> ccRecipients = null, List<Attachment> attachments = null)
        {
            throw new System.NotImplementedException();
        }
    }
}