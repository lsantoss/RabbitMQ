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
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientCellphone { get; set; }
        public bool NotifyByEmail { get; set; }
        public bool NotifyByCellphone { get; set; }
        public override byte NumberAttempts { get; set; }

        public Payment MapToPayment() => new(PaymentId, BarCode, Value, Date, ClientName, ClientEmail, ClientCellphone, NotifyByEmail, NotifyByCellphone);

        public override void AddNumberAttempt() => NumberAttempts++;
    }
}