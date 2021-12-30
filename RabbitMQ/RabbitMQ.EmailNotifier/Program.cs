using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Infra.Crosscutting;
using System;

namespace RabbitMQ.EmailNotifier
{
    class Program
    {
        private static readonly string _applicationName;
        private static readonly string _queueName;

        private static readonly IWorkerBase _workerBase;
        private static readonly IRabbitMQService _rabbitMQBus;
        //private static readonly IEmailHandler _handler;

        static Program()
        {
            _applicationName = ApplicationName.EmailNotifier;
            _queueName = QueueName.EmailNotifier;

            _workerBase = new WorkerBase(EApplication.EmailNotifier);
            _rabbitMQBus = _workerBase.GetService<IRabbitMQService>();
            //_handler = _workerBase.GetService<IEmailHandler>();
        }

        static void Main(string[] args)
        {
            Console.ReadKey();
        }
    }
}