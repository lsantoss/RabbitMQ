using System;
using System.Collections.Generic;

namespace RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories
{
    public interface IQueueLogRepository
    {
        long Log(QueueLog queueLog);
        QueueLog Get(long id);
        List<QueueLog> List(Guid paymentId);
    }
}