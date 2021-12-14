using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Domain.Reversals.Commands.Inputs;
using RabbitMQ.Domain.Reversals.Interfaces.Handlers;
using RabbitMQ.Infra.Crosscutting;
using System;

namespace RabbitMQ.ConsumerReversals
{
    class Program
    {
        private static readonly WorkerBase _workerBase;
        private static readonly IRabbitMQBus _rabbitMQBus;
        private static readonly IReversalHandler _handler;

        static Program()
        {
            _workerBase = new WorkerBase(EApplication.ConsumerReversals);
            _rabbitMQBus = _workerBase.GetService<IRabbitMQBus>();
            _handler = _workerBase.GetService<IReversalHandler>();
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Starting Worker {ApplicationName.ConsumerReversals}\n");

            _rabbitMQBus.Consume<PublishReversalCommand>(_handler, QueueName.Reversals);
        }
    }
}