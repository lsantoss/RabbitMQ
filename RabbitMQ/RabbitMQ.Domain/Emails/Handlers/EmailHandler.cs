using RabbitMQ.Domain.Common.Handlers;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.Emails.Interfaces.Services;
using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Emails.Commands.Inputs;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Emails.Helpers;
using RabbitMQ.Domain.Emails.Interfaces.Handlers;
using RabbitMQ.Domain.Payments.Entities;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Emails.Handlers
{
    public class EmailHandler : BaseHandler, IEmailHandler
    {
        private readonly string _applicationName = ApplicationName.EmailNotifier;
        private readonly string _currentQueue = QueueName.EmailNotifier;
        private readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory;

        private readonly IEmailSenderService _emailSenderService;
        private readonly IPaymentRepository _paymentRepository;

        public EmailHandler(IRabbitMQService rabbitMQBus,
                            IQueueLogRepository queueLogRepository,
                            IElmahRepository elmahRepository,
                            IPaymentRepository paymentRepository,
                            IEmailSenderService emailSenderService) : base(rabbitMQBus, queueLogRepository, elmahRepository)
        {
            _paymentRepository = paymentRepository;
            _emailSenderService = emailSenderService;
        }

        public async Task HandleAsync(EmailCommand emailCommand)
        {
            Console.WriteLine("Handle started.");

            try
            {
                var paymentQueryResult = await _paymentRepository.GetAsync(emailCommand.PaymentId);

                var emailContent = EmailHelper.GenerateEmailContent(emailCommand.EmailTemplate, paymentQueryResult);

                await _emailSenderService.SendEmailAsync(
                    emailContent,
                    "Pagamento realizado", 
                    new List<(string, string)>() { (paymentQueryResult.ClientEmail, paymentQueryResult.ClientName) } );

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
    }
}