using System;

namespace RabbitMQ.Domain.Reversals.Commands.Inputs
{
    public class ReversalCommand
    {
        public Guid Id { get; set; }
        public byte NumberAttempts { get; set; }

        public void AddNumberAttempt() => NumberAttempts++;
    }
}