using System;

namespace RabbitMQ.Domain.Reversals.Commands.Inputs
{
    public class PublishReversalCommand
    {
        public Guid Id { get; set; }
    }
}