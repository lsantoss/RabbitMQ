using System;

namespace RabbitMQ.Domain.Reversals.Queries.Results
{
    public class ReversalQueryResult
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreationDate { get; set; }
    }
}