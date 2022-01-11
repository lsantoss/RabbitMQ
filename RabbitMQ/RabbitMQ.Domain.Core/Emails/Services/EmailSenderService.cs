using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Emails.Interfaces.Services;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Core.Emails.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly SmtpClient _smtpClient;

        public EmailSenderService(SmtpSettings smtpSettings)
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

        public async Task SendEmailAsync(string content,
                                         string subject,
                                         string from,
                                         List<string> recipients,
                                         List<string> ccRecipients = null,
                                         List<Attachment> attachments = null,
                                         MailPriority priority = MailPriority.Normal)
        {
            var message = new MailMessage
            {
                Body = content,
                Subject = subject,
                From = new MailAddress(from),

                Priority = priority,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                HeadersEncoding = Encoding.UTF8
            };

            if (recipients != null && recipients.Count > 0)
                foreach (var recipient in recipients)
                    message.To.Add(new MailAddress(recipient));

            if (ccRecipients != null && ccRecipients.Count > 0)
                foreach (var ccRecipient in ccRecipients)
                    message.CC.Add(new MailAddress(ccRecipient));

            if (attachments != null && attachments.Count > 0)
                foreach (var attachment in attachments)
                    message.Attachments.Add(attachment);

            await _smtpClient.SendMailAsync(message);
        }
    }
}