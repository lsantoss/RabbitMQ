﻿using RabbitMQ.Domain.Core.AppSettings;
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
        private readonly string _fromAdress;
        private readonly SmtpClient _smtpClient;

        public EmailSenderService(SmtpSettings smtpSettings)
        {
            _fromAdress = smtpSettings.UserName;
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
                                         List<(string address, string displayName)> recipients,
                                         List<Attachment> attachments = null)
        {
            var message = new MailMessage
            {
                Body = content,
                Subject = subject,
                From = new MailAddress(_fromAdress, "LS Code", Encoding.UTF8),

                IsBodyHtml = true,
                Priority = MailPriority.Normal,

                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                HeadersEncoding = Encoding.UTF8
            };

            if (recipients != null && recipients.Count > 0)
                foreach (var (address, displayName) in recipients)
                    message.To.Add(new MailAddress(address, displayName, Encoding.UTF8));

            if (attachments != null && attachments.Count > 0)
                foreach (var attachment in attachments)
                    message.Attachments.Add(attachment);

            await _smtpClient.SendMailAsync(message);
        }
    }
}