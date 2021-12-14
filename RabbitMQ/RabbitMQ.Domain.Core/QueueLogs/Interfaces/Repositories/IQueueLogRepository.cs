using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories
{
    public interface IQueueLogRepository
    {
        Task<long> Log(QueueLog queueLog);
        Task<QueueLog> Get(long id);
        Task<List<QueueLog>> List(Guid paymentId);
    }
}