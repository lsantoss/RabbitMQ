using RabbitMQ.Domain.Common.Commands.Inputs;
using RabbitMQ.Domain.Payments.Entities;
using System;

namespace RabbitMQ.Domain.Payments.Commands.Inputs
{
    public class PaymentCommand : Command
    {
        public override Guid PaymentId { get; set; }
        public string BarCode { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
        public string Email { get; set; }
        public override byte NumberAttempts { get; set; }

        public Payment MapToPayment() => new(PaymentId, BarCode, Value, Date, Email);

        public override void AddNumberAttempt() => NumberAttempts++;
    }
}