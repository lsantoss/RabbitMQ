using System;

namespace RabbitMQ.Domain.Emails.Entities
{
    public class Email
    {
        public Guid Id { get; private set; }
        public Guid PaymentId { get; private set; }
        public bool Notified { get; private set; }
        public DateTime CreationDate { get; private set; }

        public Email(Guid paymentId, bool notified = true)
        {
            SetId(Guid.NewGuid());
            SetPaymentId(paymentId);
            SetNotified(notified);
            SetCreationDate(DateTime.Now);
        }

        public void SetId(Guid id) => Id = id;
        public void SetPaymentId(Guid paymentId) => PaymentId = paymentId;
        public void SetNotified(bool notified) => Notified = notified;
        public void SetCreationDate(DateTime creationDate) => CreationDate = creationDate;
    }
}