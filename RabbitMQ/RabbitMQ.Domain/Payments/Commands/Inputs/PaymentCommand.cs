using RabbitMQ.Domain.Payments.Entities;
using System;

namespace RabbitMQ.Domain.Payments.Commands.Inputs
{
    public class PaymentCommand
    {
        public Guid Id { get; set; }
        public string BarCode { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
        public string Email { get; set; }
        public byte NumberAttempts { get; set; }

        public Payment MapToPayment() => new Payment(Id, BarCode, Value, Date, Email);

        public void AddNumberAttempt() => NumberAttempts++;
    }
}