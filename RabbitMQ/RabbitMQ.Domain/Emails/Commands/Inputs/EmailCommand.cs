﻿using RabbitMQ.Domain.Common.Commands.Inputs;
using RabbitMQ.Domain.Core.QueueLogs.Queries.Results;
using RabbitMQ.Domain.Emails.Enums;
using System;
using System.Collections.Generic;

namespace RabbitMQ.Domain.Emails.Commands.Inputs
{
    public class EmailCommand : NotificationCommand
    {
        public override byte NumberAttempts { get; set; }
        public override Guid PaymentId { get; set; }
        public EEmailTemplate EmailTemplate { get; set; }
        public List<QueueLogQueryResult> QueueLogs { get; set; }

        public EmailCommand(Guid paymentId, EEmailTemplate emailTemplate, List<QueueLogQueryResult> queueLogs = null)
        {
            NumberAttempts = 1;
            PaymentId = paymentId;
            EmailTemplate = emailTemplate;
            QueueLogs = queueLogs ?? new List<QueueLogQueryResult>();
        }

        public override void AddNumberAttempt() => NumberAttempts++;
    }
}