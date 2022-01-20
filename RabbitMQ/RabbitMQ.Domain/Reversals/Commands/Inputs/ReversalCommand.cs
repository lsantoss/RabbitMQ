using RabbitMQ.Domain.Common.Commands.Inputs;
using System;

namespace RabbitMQ.Domain.Reversals.Commands.Inputs
{
    public class ReversalCommand : Command
    {
        public override Guid PaymentId { get; set; }
        public DateTime Date { get; set; }
        public override byte NumberAttempts { get; set; }

        public override void AddNumberAttempt() => NumberAttempts++;
    }
}