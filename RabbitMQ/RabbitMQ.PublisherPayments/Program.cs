using ElmahCore;
using Newtonsoft.Json;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Core.QueueLogs;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using RabbitMQ.Infra.Crosscutting;
using System;

namespace RabbitMQ.PublisherPayments
{
    class Program
    {
        private static readonly WorkerBase _workerBase;
        private static readonly Settings _settings;
        private static readonly QueuesWorkersSettings _queuesWorkers;
        private static readonly IQueueLogRepository _queueLogRepository;
        private static readonly IElmahRepository _elmahRepository;
        private static readonly IRabbitMQBus _rabbitMQBus;

        static Program()
        {
            _workerBase = new WorkerBase(EQueue.PublisherPayments);
            _settings = _workerBase.GetService<Settings>();
            _queuesWorkers = _workerBase.GetService<QueuesWorkersSettings>();
            _queueLogRepository = _workerBase.GetService<IQueueLogRepository>();
            _elmahRepository = _workerBase.GetService<IElmahRepository>();
            _rabbitMQBus = _workerBase.GetService<IRabbitMQBus>();
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Starting Worker {_settings.ApplicationName} \n");

                var filePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\payload.json";

                var paymentJson = FileReaderHelper.Read(filePath);

                Console.WriteLine(paymentJson);

                var payment = JsonConvert.DeserializeObject<PublishPaymentCommand>(paymentJson);

                _rabbitMQBus.Publish(payment, _queuesWorkers.PublisherPayments);

                var queueLog = new QueueLog(payment.Id, _settings.ApplicationName, _queuesWorkers.PublisherPayments, paymentJson);
                _queueLogRepository.Log(queueLog);

                Console.Write("\nMessage send with success!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                _elmahRepository.Log(new Error(ex));
                Console.Write($"\nError sending message! {ex.Message}");
                Console.ReadKey();
            }
        }
    }
}