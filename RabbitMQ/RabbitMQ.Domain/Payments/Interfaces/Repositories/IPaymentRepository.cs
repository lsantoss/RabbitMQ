using RabbitMQ.Domain.Payments.Entities;
using RabbitMQ.Domain.Payments.Queries.Results;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Payments.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task Save(Payment payment);
        Task Update(Payment payment);
        Task<PaymentQueryResult> Get(Guid id);
    }
}