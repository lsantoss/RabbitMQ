using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using RabbitMQ.Domain.Payments.Interfaces.Handlers;
using RabbitMQ.Infra.Crosscutting;
using System;

namespace RabbitMQ.ConsumerPayments
{
    class Program
    {
        private static readonly string _applicationName;
        private static readonly string _queueName;

        private static readonly WorkerBase _workerBase;
        private static readonly IRabbitMQBus _rabbitMQBus;
        private static readonly IPaymentHandler _handler;

        static Program()
        {
            _applicationName = ApplicationName.PublisherPayments;
            _queueName = QueueName.Payments;

            _workerBase = new WorkerBase(EApplication.ConsumerPayments);
            _rabbitMQBus = _workerBase.GetService<IRabbitMQBus>();
            _handler = _workerBase.GetService<IPaymentHandler>();
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Starting Worker {_applicationName}\n");

            _rabbitMQBus.Consume<PublishPaymentCommand>(_handler, _queueName);
        }
    }
}