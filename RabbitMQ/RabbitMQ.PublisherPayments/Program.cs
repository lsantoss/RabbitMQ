﻿using Newtonsoft.Json;
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
        private static readonly string _applicationName = ApplicationName.PublisherPayments;
        private static readonly string _queueName = QueueName.Payments;
        private static readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory;

        private static readonly IWorkerBase _workerBase;
        private static readonly IQueueLogRepository _queueLogRepository;
        private static readonly IElmahRepository _elmahRepository;
        private static readonly IRabbitMQService _rabbitMQService;

        static Program()
        {
            _workerBase = new WorkerBase(EApplication.PublisherPayments);
            _queueLogRepository = _workerBase.GetService<IQueueLogRepository>();
            _elmahRepository = _workerBase.GetService<IElmahRepository>();
            _rabbitMQService = _workerBase.GetService<IRabbitMQService>();
        }

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Starting Worker {_applicationName} \n");

                var paymentCommandJson = FileReaderHelper.Read($@"{_basePath}\payload.json");

                Console.WriteLine(paymentCommandJson);

                var paymentCommand = JsonConvert.DeserializeObject<PaymentCommand>(paymentCommandJson);

                _rabbitMQService.Publish(paymentCommand, _queueName);

                var queueLog = new QueueLog(paymentCommand.PaymentId, _applicationName, _queueName, paymentCommandJson);
                _ = await _queueLogRepository.LogAsync(queueLog);

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
    }
}