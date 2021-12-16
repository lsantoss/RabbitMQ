using RabbitMQ.Domain.Core.QueueLogs.Queries.Results;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using RabbitMQ.Domain.Payments.Entities;
using System.Collections.Generic;

namespace RabbitMQ.Domain.Emails.Commands.Inputs
{
    public class EmailNotificationCommand
    {
        public Payment Payment { get; private set; }
        public EEmailTemplate EmailTemplate { get; private set; }
        public List<QueueLogQueryResult> QueueLogs { get; private set; }

        public EmailNotificationCommand() { }

        public EmailNotificationCommand(Payment payment, EEmailTemplate emailTemplate)
        {
            Payment = payment;
            EmailTemplate = emailTemplate;
            QueueLogs = new List<QueueLogQueryResult>();
        }

        public EmailNotificationCommand(PublishPaymentCommand paymentCommand, EEmailTemplate emailTemplate, List<QueueLogQueryResult> queueLogs)
        {
            Payment = paymentCommand.MapToPayment();
            EmailTemplate = emailTemplate;
            QueueLogs = queueLogs;
        }
    }
}