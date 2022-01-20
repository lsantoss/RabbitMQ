using System;

namespace RabbitMQ.Domain.Reversals.Entities
{
    public class Reversal
    {
        public Guid Id { get; private set; }
        public Guid PaymentId { get; private set; }
        public DateTime Date { get; private set; }
        public DateTime CreationDate { get; private set; }

        public Reversal(Guid paymentId, DateTime date)
        {
            SetId(Guid.NewGuid());
            SetPaymentId(paymentId);
            SetDate(date);
            SetCreationDate(DateTime.Now);
        }

        public void SetId(Guid id) => Id = id;
        public void SetPaymentId(Guid paymentId) => PaymentId = paymentId;
        public void SetDate(DateTime date) => Date = date;
        public void SetCreationDate(DateTime creationDate) => CreationDate = creationDate;
    }
}