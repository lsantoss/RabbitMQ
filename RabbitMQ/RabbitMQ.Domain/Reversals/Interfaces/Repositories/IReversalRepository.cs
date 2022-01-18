using RabbitMQ.Domain.Reversals.Entities;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Reversals.Interfaces.Repositories
{
    public interface IReversalRepository
    {
        Task SaveAsync(Reversal reversal);
    }
}