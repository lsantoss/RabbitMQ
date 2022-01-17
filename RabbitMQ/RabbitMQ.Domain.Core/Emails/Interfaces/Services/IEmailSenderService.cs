using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Core.Emails.Interfaces.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string content,
                            string subject,
                            (string address, string displayName) recipient,
                            List<Attachment> attachments = null);
    }
}