using RabbitMQ.Domain.Payments.Entities;
using System;

namespace RabbitMQ.Domain.Payments.Queries.Results
{
    public class PaymentQueryResult
    {
        public Guid Id { get; set; }
        public string BarCode { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
        public string Email { get; set; }
        public bool Reversed { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ChangeDate { get; set; }

        public Payment MapToPayment() => new(Id, BarCode, Value, Date, Email, Reversed, CreationDate, ChangeDate);
    }
}