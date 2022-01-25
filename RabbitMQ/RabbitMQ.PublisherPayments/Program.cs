using Newtonsoft.Json;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Core.QueueLogs.Entities;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using RabbitMQ.Infra.Crosscutting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitMQ.PublisherPayments
{
    class Program
    {
        private static readonly string _queueName = QueueName.Payments;
        private static readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string _applicationName = AppDomain.CurrentDomain.FriendlyName;

        private static readonly IWorkerBase _workerBase;
        private static readonly IQueueLogRepository _queueLogRepository;
        private static readonly IElmahRepository _elmahRepository;
        private static readonly IRabbitMQService _rabbitMQService;

        static Program()
        {
            _workerBase = new WorkerBase();
            _queueLogRepository = _workerBase.GetService<IQueueLogRepository>();
            _elmahRepository = _workerBase.GetService<IElmahRepository>();
            _rabbitMQService = _workerBase.GetService<IRabbitMQService>();
        }

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Starting Worker {_applicationName} \n");

                var paymentCommandJson = FileHelper.Read($@"{_basePath}\payload.json");

                Console.WriteLine(paymentCommandJson);

                var paymentCommand = JsonConvert.DeserializeObject<PaymentCommand>(paymentCommandJson);

                _rabbitMQService.Publish(paymentCommand, _queueName);

                var queueLog = new QueueLog(paymentCommand.PaymentId, _applicationName, _queueName, paymentCommandJson);
                _ = await _queueLogRepository.LogAsync(queueLog);

                UpdateAnotherJsonFiles(paymentCommand.PaymentId);

                Console.Write("\nMessage send with success!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                _ = await _elmahRepository.LogAsync(ex);
                Console.Write($"\nError sending message! {ex.Message}");
                Console.ReadKey();
            }
        }

        private static void UpdateAnotherJsonFiles(Guid paymentId)
        {
            var basePathPublishReversal = _basePath.Replace(@"RabbitMQ.PublisherPayments\bin\Debug\net5.0\", "RabbitMQ.PublisherReversals");
            var basePathPublisherEmails = _basePath.Replace(@"RabbitMQ.PublisherPayments\bin\Debug\net5.0\", "RabbitMQ.PublisherEmails");

            var listPaths = new List<string>()
            {
                $@"{basePathPublishReversal}\payload.json",
                $@"{basePathPublisherEmails}\payload-payment-success.json",
                $@"{basePathPublisherEmails}\payload-reversal-success.json",
                $@"{basePathPublisherEmails}\payload-support-payment-already-reversed.json",
                $@"{basePathPublisherEmails}\payload-support-payment-maximum-attempts.json",
                $@"{basePathPublisherEmails}\payload-support-payment-not-found-for-reversal.json",
                $@"{basePathPublisherEmails}\payload-support-reversal-maximum-attempts.json"
            };

            foreach (var path in listPaths)
            {
                var payload = FileHelper.ReadJsonToDynamicObject(path);
                payload["PaymentId"] = paymentId.ToString();
                FileHelper.SaveObjectToJson(path, payload);
            }
        }
    }
}