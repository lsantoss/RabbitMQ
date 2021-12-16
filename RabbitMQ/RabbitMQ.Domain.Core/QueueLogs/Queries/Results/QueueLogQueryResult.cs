using System;

namespace RabbitMQ.Domain.Core.QueueLogs.Queries.Results
{
    public class QueueLogQueryResult
    {
        public ulong Id { get; set; }
        public Guid PaymentId { get; set; }
        public string Worker { get; set; }
        public string Queue { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool Success { get; set; }
        public byte NumberAttempts { get; set; }
        public string Error { get; set; }

        public QueueLog MapToQueueLog() => new QueueLog(Id, PaymentId, Worker, Queue, Message, Date, Success, NumberAttempts, Error);
    }
}