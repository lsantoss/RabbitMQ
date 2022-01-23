﻿using RabbitMQ.Domain.Common.Handlers;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.Emails.Interfaces.Services;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Emails.Commands.Inputs;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Emails.Helpers;
using RabbitMQ.Domain.Emails.Interfaces.Handlers;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using RabbitMQ.Domain.Payments.Queries.Results;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Emails.Handlers
{
    public class EmailHandler : BaseHandler, IEmailHandler
    {
        private readonly string _currentQueue = QueueName.EmailNotifier;
        private readonly string _applicationName = AppDomain.CurrentDomain.FriendlyName;

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

                VerifyPaymentData(paymentQueryResult, emailCommand.EmailTemplate);

                var templateHtml = EmailHelper.GenerateTemplate(emailCommand.EmailTemplate);

                var emailContent = EmailHelper.ChangeKeysForValues(emailCommand.EmailTemplate, templateHtml, paymentQueryResult, emailCommand.QueueLogs);

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

        private static void VerifyPaymentData(PaymentQueryResult paymentQueryResult, EEmailTemplate emailTemplate)
        {
            if (paymentQueryResult == null)
            {
                if (emailTemplate != EEmailTemplate.SupportPaymentMaximumAttempts &&
                    emailTemplate != EEmailTemplate.SupportPaymentNotFoundForReversal)
                {
                    Console.WriteLine("An error has occurred. This payment is not registered in our database.");
                    Console.WriteLine("Handle finished.\n");
                    return;
                }
            }
        }
    }
}