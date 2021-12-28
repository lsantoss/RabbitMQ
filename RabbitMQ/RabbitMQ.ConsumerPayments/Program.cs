using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
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

        private static readonly IWorkerBase _workerBase;
        private static readonly IRabbitMQService _rabbitMQBus;
        private static readonly IPaymentHandler _handler;

        static Program()
        {
            _applicationName = ApplicationName.ConsumerPayments;
            _queueName = QueueName.Payments;

            _workerBase = new WorkerBase(EApplication.ConsumerPayments);
            _rabbitMQBus = _workerBase.GetService<IRabbitMQService>();
            _handler = _workerBase.GetService<IPaymentHandler>();
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Starting Worker {_applicationName}\n");

            _rabbitMQBus.Consume<PaymentCommand>(_handler, _queueName);
        }
    }
}