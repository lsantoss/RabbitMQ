using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Emails.Commands.Inputs;
using RabbitMQ.Domain.Emails.Interfaces.Handlers;
using RabbitMQ.Infra.Crosscutting;
using System;

namespace RabbitMQ.EmailNotifier
{
    class Program
    {
        private static readonly string _applicationName = ApplicationName.EmailNotifier;
        private static readonly string _queueName = QueueName.EmailNotifier;

        private static readonly IWorkerBase _workerBase;
        private static readonly IRabbitMQService _rabbitMQService;
        private static readonly IEmailHandler _handler;

        static Program()
        {
            _workerBase = new WorkerBase(EApplication.EmailNotifier);
            _rabbitMQService = _workerBase.GetService<IRabbitMQService>();
            _handler = _workerBase.GetService<IEmailHandler>();
        }

        static void Main(string[] args)
        {
            Console.WriteLine($"Starting Worker {_applicationName}\n");

            _handler.HandleAsync(null).GetAwaiter().GetResult();

            //_rabbitMQService.Consume<EmailCommand>(_handler, _queueName);
        }
    }
}