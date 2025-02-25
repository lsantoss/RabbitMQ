﻿using RabbitMQ.Domain.Core.Extensions;
using System;

namespace RabbitMQ.Domain.Core.QueueLogs.Entities
{
    public class QueueLog
    {
        public ulong Id { get; private set; }
        public Guid PaymentId { get; private set; }
        public string Worker { get; private set; }
        public string Queue { get; private set; }
        public string Message { get; private set; }
        public DateTime Date { get; private set; }
        public bool Success { get; private set; }
        public byte NumberAttempts { get; private set; }
        public string Error { get; private set; }

        public QueueLog(Guid paymentId, string worker, string queue, string message, bool success = true, byte numberAttempts = 1, string error = null)
        {
            SetId(0);
            SetPaymentId(paymentId);
            SetWorker(worker);
            SetQueue(queue);
            SetMessage(message.RemoveJsonFormatting());
            SetDate(DateTime.Now);
            SetSuccess(success);
            SetNumberAttempts(numberAttempts);
            SetError(error);
        }

        public void SetId(ulong id) => Id = id;
        public void SetPaymentId(Guid paymentId) => PaymentId = paymentId;
        public void SetWorker(string worker) => Worker = worker;
        public void SetQueue(string queue) => Queue = queue;
        public void SetMessage(string message) => Message = message.RemoveJsonFormatting();
        public void SetDate(DateTime date) => Date = date;
        public void SetSuccess(bool success) => Success = success;
        public void SetNumberAttempts(byte numberAttempts) => NumberAttempts = numberAttempts;
        public void SetError(string error) => Error = error;
    }
}