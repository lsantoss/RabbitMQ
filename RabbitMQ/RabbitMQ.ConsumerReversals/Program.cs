using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Reversals.Commands.Inputs;
using RabbitMQ.Domain.Reversals.Interfaces.Handlers;
using RabbitMQ.Infra.Crosscutting;
using System;

namespace RabbitMQ.ConsumerReversals
{
    class Program
    {
        private static readonly string _applicationName;
        private static readonly string _queueName;

        private static readonly WorkerBase _workerBase;
        private static readonly IRabbitMQService _rabbitMQBus;
        private static readonly IReversalHandler _handler;

        static Program()
        {
            _applicationName = ApplicationName.ConsumerReversals;
            _queueName = QueueName.Reversals;

            _workerBase = new WorkerBase(EApplication.ConsumerReversals);
            _rabbitMQBus = _workerBase.GetService<IRabbitMQService>();
            _handler = _workerBase.GetService<IReversalHandler>();
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Starting Worker {_applicationName}\n");

            _rabbitMQBus.Consume<ReversalCommand>(_handler, _queueName);
        }
    }
}