using Newtonsoft.Json;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Core.QueueLogs.Entities;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using RabbitMQ.Domain.Reversals.Commands.Inputs;
using RabbitMQ.Infra.Crosscutting;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.PublisherReversals
{
    class Program
    {
        private static readonly string _applicationName;
        private static readonly string _queueName;
        private static readonly string _basePath;

        private static readonly IWorkerBase _workerBase;
        private static readonly IQueueLogRepository _queueLogRepository;
        private static readonly IElmahRepository _elmahRepository;
        private static readonly IRabbitMQService _rabbitMQService;
        private static readonly IPaymentRepository _paymentRepository;

        static Program()
        {
            _applicationName = ApplicationName.PublisherReversals;
            _queueName = QueueName.Reversals;
            _basePath = AppDomain.CurrentDomain.BaseDirectory;

            _workerBase = new WorkerBase(EApplication.PublisherReversals);
            _queueLogRepository = _workerBase.GetService<IQueueLogRepository>();
            _elmahRepository = _workerBase.GetService<IElmahRepository>();
            _rabbitMQService = _workerBase.GetService<IRabbitMQService>();
            _paymentRepository = _workerBase.GetService<IPaymentRepository>();
        }

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Starting Worker {_applicationName} \n");

                var filePath = $@"{_basePath}\payload.json";

                var reversalCommandJson = FileReaderHelper.Read(filePath);

                Console.WriteLine(reversalCommandJson);

                var reversalCommand = JsonConvert.DeserializeObject<ReversalCommand>(reversalCommandJson);

                var paymetQueryResult = await _paymentRepository.GetAsync(reversalCommand.PaymentId);

                if (paymetQueryResult == null)
                {
                    Console.Write("\nAn error has occurred. This payment is not registered in our database.");
                    Console.ReadKey();
                    return;
                }

                _rabbitMQService.Publish(reversalCommand, _queueName);

                var queueLog = new QueueLog(reversalCommand.PaymentId, _applicationName, _queueName, reversalCommandJson);
                await _queueLogRepository.LogAsync(queueLog);

                Console.Write("\nMessage send with success!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                await _elmahRepository.LogAsync(ex);
                Console.Write($"\nError sending message! {ex.Message}");
                Console.ReadKey();
            }
        }
    }
}