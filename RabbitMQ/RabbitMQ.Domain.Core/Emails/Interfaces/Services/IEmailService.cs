using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Core.Emails.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string content, string subject, string from, List<string> recipients, List<string> ccRecipients = null, 
                            List<Attachment> attachments = null, MailPriority priority = MailPriority.Normal);
    }
}