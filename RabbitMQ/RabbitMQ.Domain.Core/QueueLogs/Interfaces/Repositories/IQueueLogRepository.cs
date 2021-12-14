using System.Threading.Tasks;

namespace RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories
{
    public interface IQueueLogRepository
    {
        Task<long> Log(QueueLog queueLog);
    }
}