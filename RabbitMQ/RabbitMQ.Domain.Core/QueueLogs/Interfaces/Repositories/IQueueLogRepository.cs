using RabbitMQ.Domain.Core.QueueLogs.Entities;
using RabbitMQ.Domain.Core.QueueLogs.Queries.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories
{
    public interface IQueueLogRepository
    {
        Task<ulong> LogAsync(QueueLog queueLog);
        Task<List<QueueLogQueryResult>> ListAsync(Guid paymentId);
    }
}