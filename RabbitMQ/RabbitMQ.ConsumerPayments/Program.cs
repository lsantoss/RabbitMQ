﻿using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using RabbitMQ.Domain.Payments.Interfaces.Handlers;
using RabbitMQ.Infra.Crosscutting;
using System;

namespace RabbitMQ.ConsumerPayments
{
    class Program
    {
        private static readonly string _queueName = QueueName.Payment;
        private static readonly string _applicationName = AppDomain.CurrentDomain.FriendlyName;

        private static readonly IWorkerBase _workerBase;
        private static readonly IRabbitMQService _rabbitMQService;
        private static readonly IPaymentHandler _handler;

        static Program()
        {
            _workerBase = new WorkerBase();
            _rabbitMQService = _workerBase.GetService<IRabbitMQService>();
            _handler = _workerBase.GetService<IPaymentHandler>();
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Starting Worker {_applicationName}\n");

            _rabbitMQService.Consume<PaymentCommand>(_handler, _queueName);
        }
    }
}