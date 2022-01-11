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
        private readonly string _applicationName;
        private readonly string _currentQueue;
        private readonly string _basePath;

        private readonly IEmailSenderService _emailSenderService;

        public EmailHandler(IRabbitMQService rabbitMQBus,
                            IQueueLogRepository queueLogRepository,
                            IElmahRepository elmahRepository,
                            IEmailSenderService emailSenderService) : base(rabbitMQBus, queueLogRepository, elmahRepository)
        {
            _applicationName = ApplicationName.EmailNotifier;
            _currentQueue = QueueName.EmailNotifier;
            _basePath = AppDomain.CurrentDomain.BaseDirectory;

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
            string html;

            switch (emailTemplate)
            {
                case EEmailTemplate.PaymentSuccess:
                    html = FileReaderHelper.Read($@"{_basePath}\Emails\Resources\HTMLs\payment-success.html");
                    break;

                case EEmailTemplate.ReversalSuccess:
                    html = FileReaderHelper.Read($@"{_basePath}");
                    break;

                case EEmailTemplate.SupportPaymentMaximumAttempts:
                    html = FileReaderHelper.Read($@"{_basePath}");
                    break;

                case EEmailTemplate.SupportReversalMaximumAttempts:
                    html = FileReaderHelper.Read($@"{_basePath}");
                    break;

                case EEmailTemplate.SupportPaymentNotFoundForReversal:
                    html = FileReaderHelper.Read($@"{_basePath}");
                    break;

                case EEmailTemplate.SupportPaymentAlreadyReversed:
                    html = FileReaderHelper.Read($@"{_basePath}");
                    break;

                default:
                    html = null;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(html))
            {
                var css = FileReaderHelper.Read($@"{_basePath}\Emails\Resources\CSSs\style.css");

                html = html.Replace("{#style.css#}", $"<style>{css}</style>");
            }

            return html;
        }
    }
}