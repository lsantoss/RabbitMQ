using RabbitMQ.Domain.Reversals.Entities;
using RabbitMQ.Domain.Reversals.Queries.Results;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Reversals.Interfaces.Repositories
{
    public interface IReversalRepository
    {
        Task SaveAsync(Reversal reversal);
        Task<ReversalQueryResult> GetAsync(Guid paymentId);
    }
}