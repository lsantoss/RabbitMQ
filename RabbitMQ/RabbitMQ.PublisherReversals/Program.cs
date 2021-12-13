using ElmahCore;
using Newtonsoft.Json;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Core.QueueLogs;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Domain.Reversals.Commands.Inputs;
using RabbitMQ.Infra.Crosscutting;
using System;

namespace RabbitMQ.PublisherReversals
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
            _workerBase = new WorkerBase(EQueue.PublisherReversals);
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

                var reversalJson = FileReaderHelper.Read(filePath);

                Console.WriteLine(reversalJson);

                var reversal = JsonConvert.DeserializeObject<PublishReversalCommand>(reversalJson);

                _rabbitMQBus.Publish(reversal, _queuesWorkers.PublisherReversals);

                var queueLog = new QueueLog(reversal.Id, _settings.ApplicationName, _queuesWorkers.PublisherPayments, reversalJson);
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