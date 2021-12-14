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
        private static readonly WorkerBase _workerBase;
        private static readonly IRabbitMQBus _rabbitMQBus;
        private static readonly IPaymentHandler _handler;

        static Program()
        {
            _workerBase = new WorkerBase(EQueue.ConsumerPayments);
            _rabbitMQBus = _workerBase.GetService<IRabbitMQBus>();
            _handler = _workerBase.GetService<IPaymentHandler>();
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Starting Worker {ApplicationName.ConsumerPayments}\n");

            _rabbitMQBus.Consume<PublishPaymentCommand>(_handler, QueueName.Payments);
        }
    }
}