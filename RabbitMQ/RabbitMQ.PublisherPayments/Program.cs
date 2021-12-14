﻿using ElmahCore;
using Newtonsoft.Json;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Core.QueueLogs;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using RabbitMQ.Infra.Crosscutting;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.PublisherPayments
{
    class Program
    {
        private static readonly WorkerBase _workerBase;
        private static readonly IQueueLogRepository _queueLogRepository;
        private static readonly IElmahRepository _elmahRepository;
        private static readonly IRabbitMQBus _rabbitMQBus;

        static Program()
        {
            _workerBase = new WorkerBase(EQueue.PublisherPayments);
            _queueLogRepository = _workerBase.GetService<IQueueLogRepository>();
            _elmahRepository = _workerBase.GetService<IElmahRepository>();
            _rabbitMQBus = _workerBase.GetService<IRabbitMQBus>();
        }

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Starting Worker {ApplicationName.PublisherPayments} \n");

                var filePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\payload.json";

                var paymentJson = FileReaderHelper.Read(filePath);

                Console.WriteLine(paymentJson);

                var payment = JsonConvert.DeserializeObject<PublishPaymentCommand>(paymentJson);

                _rabbitMQBus.Publish(payment, QueueName.Payments);

                var queueLog = new QueueLog(payment.Id, ApplicationName.PublisherPayments, QueueName.Payments, paymentJson);
                await _queueLogRepository.Log(queueLog);

                Console.Write("\nMessage send with success!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                await _elmahRepository.Log(new Error(ex));
                Console.Write($"\nError sending message! {ex.Message}");
                Console.ReadKey();
            }
        }
    }
}