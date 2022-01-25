using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Reversals.Commands.Inputs;
using RabbitMQ.Domain.Reversals.Interfaces.Handlers;
using RabbitMQ.Infra.Crosscutting;
using System;

namespace RabbitMQ.ConsumerReversals
{
    class Program
    {
        private static readonly string _queueName = QueueName.Reversals;
        private static readonly string _applicationName = AppDomain.CurrentDomain.FriendlyName;

        private static readonly IWorkerBase _workerBase;
        private static readonly IRabbitMQService _rabbitMQService;
        private static readonly IReversalHandler _handler;

        static Program()
        {
            _workerBase = new WorkerBase();
            _rabbitMQService = _workerBase.GetService<IRabbitMQService>();
            _handler = _workerBase.GetService<IReversalHandler>();
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Starting Worker {_applicationName}\n");

            _rabbitMQService.Consume<ReversalCommand>(_handler, _queueName);
        }
    }
}