using System;

namespace RabbitMQ.Domain.Core.QueueLogs
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

        public QueueLog(Guid paymentId, string worker, string queue, string message)
        {
            Id = 0;
            PaymentId = paymentId;
            Worker = worker;
            Queue = queue;
            Message = message;
            Date = DateTime.Now;
            Success = true;
            NumberAttempts = 1;
            Error = null;
        }

        public QueueLog(Guid paymentId, string worker, string queue, string message, byte numberAttempts, string error)
        {
            Id = 0;
            PaymentId = paymentId;
            Worker = worker;
            Queue = queue;
            Message = message;
            Date = DateTime.Now;
            Success = false;
            NumberAttempts = numberAttempts;
            Error = error;
        }
    }
}