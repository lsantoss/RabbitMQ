using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.QueueLogs.Queries.Results;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using RabbitMQ.Domain.Payments.Entities;
using System.Collections.Generic;

namespace RabbitMQ.Domain.Emails.Commands.Inputs
{
    public class EmailNotificationCommand
    {        
        public EEmailTemplate EmailTemplate { get; private set; }
        public string EmailSupport { get; private set; }
        public byte NumberAttempts { get; private set; }
        public Payment Payment { get; private set; }
        public List<QueueLogQueryResult> QueueLogs { get; private set; }

        public EmailNotificationCommand(Payment payment, EEmailTemplate emailTemplate)
        {
            EmailTemplate = emailTemplate;
            EmailSupport = null;
            NumberAttempts = 1;
            Payment = payment;
            QueueLogs = new List<QueueLogQueryResult>();
        }

        public EmailNotificationCommand(PublishPaymentCommand paymentCommand, EEmailTemplate emailTemplate, List<QueueLogQueryResult> queueLogs)
        {
            EmailTemplate = emailTemplate;
            EmailSupport = EmailContact.Support;
            NumberAttempts = 1;
            Payment = paymentCommand.MapToPayment();
            QueueLogs = queueLogs;
        }

        public void AddNumberAttempt() => NumberAttempts++;
    }
}