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
        public EEmailTemplate EmailTemplate { get; set; }
        public string EmailSupport { get; set; }
        public byte NumberAttempts { get; set; }
        public PaymentCommand Payment { get; set; }
        public List<QueueLogQueryResult> QueueLogs { get; set; }

        public EmailNotificationCommand(PaymentCommand paymentCommand, EEmailTemplate emailTemplate, List<QueueLogQueryResult> queueLogs = null)
        {
            EmailTemplate = emailTemplate;
            EmailSupport = EmailContact.Support;
            NumberAttempts = 1;
            Payment = paymentCommand;
            QueueLogs = queueLogs != null ? queueLogs : new List<QueueLogQueryResult>();
        }

        public void AddNumberAttempt() => NumberAttempts++;
    }
}