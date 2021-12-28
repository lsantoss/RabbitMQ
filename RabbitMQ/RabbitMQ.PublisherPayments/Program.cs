using Newtonsoft.Json;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Core.QueueLogs.Entities;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using RabbitMQ.Infra.Crosscutting;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.PublisherPayments
{
    class Program
    {
        private static readonly string _applicationName;
        private static readonly string _queueName;
        private static readonly string _basePath;

        private static readonly WorkerBase _workerBase;
        private static readonly IQueueLogRepository _queueLogRepository;
        private static readonly IElmahRepository _elmahRepository;
        private static readonly IRabbitMQService _rabbitMQBus;

        static Program()
        {
            _applicationName = ApplicationName.PublisherPayments;
            _queueName = QueueName.Payments;
            _basePath = AppDomain.CurrentDomain.BaseDirectory;

            _workerBase = new WorkerBase(EApplication.PublisherPayments);
            _queueLogRepository = _workerBase.GetService<IQueueLogRepository>();
            _elmahRepository = _workerBase.GetService<IElmahRepository>();
            _rabbitMQBus = _workerBase.GetService<IRabbitMQService>();
        }

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Starting Worker {_applicationName} \n");

                var filePath = $@"{_basePath}\payload.json";

                var paymentCommandJson = FileReaderHelper.Read(filePath);

                Console.WriteLine(paymentCommandJson);

                var paymentCommand = JsonConvert.DeserializeObject<PaymentCommand>(paymentCommandJson);

                _rabbitMQBus.Publish(paymentCommand, _queueName);

                var queueLog = new QueueLog(paymentCommand.PaymentId, _applicationName, _queueName, paymentCommandJson);
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