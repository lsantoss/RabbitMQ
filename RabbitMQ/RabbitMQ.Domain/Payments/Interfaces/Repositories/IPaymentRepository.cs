using RabbitMQ.Domain.Payments.Entities;
using RabbitMQ.Domain.Payments.Queries.Results;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Payments.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task SaveAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task<PaymentQueryResult> GetAsync(Guid id);
    }
}