using RabbitMQ.Domain.Common.Handlers;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.Emails.Interfaces.Services;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Emails.Commands.Inputs;
using RabbitMQ.Domain.Emails.Helpers;
using RabbitMQ.Domain.Emails.Interfaces.Handlers;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using RabbitMQ.Domain.Reversals.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Emails.Handlers
{
    public class EmailHandler : BaseHandler, IEmailHandler
    {
        private readonly string _currentQueue = QueueName.EmailNotifier;
        private readonly string _applicationName = AppDomain.CurrentDomain.FriendlyName;

        private readonly IPaymentRepository _paymentRepository;
        private readonly IReversalRepository _reversalRepository;
        private readonly IEmailSenderService _emailSenderService;

        public EmailHandler(IRabbitMQService rabbitMQBus,
                            IQueueLogRepository queueLogRepository,
                            IElmahRepository elmahRepository,
                            IPaymentRepository paymentRepository,
                            IReversalRepository reversalRepository,
                            IEmailSenderService emailSenderService) : base(rabbitMQBus, queueLogRepository, elmahRepository)
        {
            _paymentRepository = paymentRepository;
            _reversalRepository = reversalRepository;
            _emailSenderService = emailSenderService;
        }

        public async Task HandleAsync(EmailCommand emailCommand)
        {
            Console.WriteLine("Handle started.");

            try
            {
                var paymentQueryResult = await _paymentRepository.GetAsync(emailCommand.PaymentId);
                var reversalQueryResult = await _reversalRepository.GetAsync(emailCommand.PaymentId);

                var emailContent = EmailHelper.GenerateTemplate(
                    emailCommand, paymentQueryResult, reversalQueryResult, emailCommand.QueueLogs);

                var subject = EmailHelper.GenerateSubject(emailCommand.EmailTemplate);

                var recipient = EmailHelper.GenerateRecipient(emailCommand.EmailTemplate, paymentQueryResult);

                var attachments = EmailHelper.GenerateAttachments();

                await _emailSenderService.SendEmailAsync(emailContent, subject, recipient, attachments);

                await LogQueueAsync(emailCommand, _applicationName, _currentQueue);

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