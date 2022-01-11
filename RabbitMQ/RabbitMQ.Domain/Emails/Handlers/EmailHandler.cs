using RabbitMQ.Domain.Common.Handlers;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.Emails.Interfaces.Services;
using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Emails.Commands.Inputs;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Emails.Interfaces.Handlers;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Emails.Handlers
{
    public class EmailHandler : BaseHandler, IEmailHandler
    {
        private readonly string _applicationName = ApplicationName.EmailNotifier;
        private readonly string _currentQueue = QueueName.EmailNotifier;
        private readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory;

        private readonly IEmailSenderService _emailSenderService;

        public EmailHandler(IRabbitMQService rabbitMQBus,
                            IQueueLogRepository queueLogRepository,
                            IElmahRepository elmahRepository,
                            IEmailSenderService emailSenderService) : base(rabbitMQBus, queueLogRepository, elmahRepository)
        {
            _emailSenderService = emailSenderService;
        }

        public async Task HandleAsync(EmailCommand emailCommand)
        {
            Console.WriteLine("Handle started.");

            try
            {
                var html = GetTemplate(EEmailTemplate.PaymentSuccess);

                //Notification

                //Send Email Notification

                //await LogQueueAsync(emailCommand, _applicationName, _currentQueue);

                Console.WriteLine("Email send successfully.");
            }
            catch (Exception ex)
            {
                await ControlMaximumAttemptsAsync(emailCommand, _applicationName, _currentQueue, emailCommand.EmailTemplate, ex);
            }

            Console.WriteLine("Handle finished.\n");
        }

        private string GetTemplate(EEmailTemplate emailTemplate)
        {
            string html = emailTemplate switch
            {
                EEmailTemplate.PaymentSuccess => FileReaderHelper.Read($@"{_basePath}\Emails\Resources\HTMLs\payment-success.html"),
                EEmailTemplate.ReversalSuccess => FileReaderHelper.Read($@"{_basePath}"),
                EEmailTemplate.SupportPaymentMaximumAttempts => FileReaderHelper.Read($@"{_basePath}"),
                EEmailTemplate.SupportReversalMaximumAttempts => FileReaderHelper.Read($@"{_basePath}"),
                EEmailTemplate.SupportPaymentNotFoundForReversal => FileReaderHelper.Read($@"{_basePath}"),
                EEmailTemplate.SupportPaymentAlreadyReversed => FileReaderHelper.Read($@"{_basePath}"),
                _ => null,
            };

            if (!string.IsNullOrWhiteSpace(html))
            {
                var css = FileReaderHelper.Read($@"{_basePath}\Emails\Resources\CSSs\style.css");
                html = html.Replace("{#style.css#}", $"<style>{css}</style>");
            }

            return html;
        }
    }
}