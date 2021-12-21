using Newtonsoft.Json;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Core.QueueLogs;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Domain.Emails.Commands.Inputs;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Infra.Crosscutting;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.PublisherEmails
{
    class Program
    {
        private static readonly string _applicationName;
        private static readonly string _queueName;

        private static readonly WorkerBase _workerBase;
        private static readonly IQueueLogRepository _queueLogRepository;
        private static readonly IElmahRepository _elmahRepository;
        private static readonly IRabbitMQBus _rabbitMQBus;

        static Program()
        {
            _applicationName = ApplicationName.PublisherEmails;
            _queueName = QueueName.EmailNotifier;

            _workerBase = new WorkerBase(EApplication.PublisherEmails);
            _queueLogRepository = _workerBase.GetService<IQueueLogRepository>();
            _elmahRepository = _workerBase.GetService<IElmahRepository>();
            _rabbitMQBus = _workerBase.GetService<IRabbitMQBus>();
        }

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Starting Worker {_applicationName} \n");

                PrintMenu();

                Console.Write("Enter the desired template: ");
                var emailTemplate = (EEmailTemplate)Convert.ToInt16(Console.ReadLine());

                var emailJson = ReadPayload(emailTemplate);

                Console.WriteLine();
                Console.WriteLine(emailJson);

                var emailNotification = JsonConvert.DeserializeObject<EmailNotificationCommand>(emailJson);

                _rabbitMQBus.Publish(emailJson, _queueName);

                var queueLog = new QueueLog(emailNotification.Payment.Id, _applicationName, _queueName, emailJson);
                await _queueLogRepository.Log(queueLog);

                Console.Write("\nMessage send with success!");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                await _elmahRepository.Log(ex);
                Console.Write($"\nError sending message! {ex.Message}");
                Console.ReadKey();
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("==========================================");
            Console.WriteLine("1 - Payment Success");
            Console.WriteLine("2 - Reversal Success");
            Console.WriteLine("3 - Support Payment Maximum Attempts");
            Console.WriteLine("4 - Support Reversal Maximum Attempts");
            Console.WriteLine("5 - Support Payment Not Found For Reversal");
            Console.WriteLine("6 - Support Payment Already Reversed");
            Console.WriteLine("==========================================");
            Console.WriteLine();
        }

        private static string ReadPayload(EEmailTemplate emailTemplate)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            switch (emailTemplate)
            {
                case EEmailTemplate.PaymentSuccess:
                    return FileReaderHelper.Read($@"{basePath}\payload-payment-success.json");

                case EEmailTemplate.ReversalSuccess:
                    return FileReaderHelper.Read($@"{basePath}\payload-reversal-success.json");

                case EEmailTemplate.SupportPaymentMaximumAttempts:
                    return FileReaderHelper.Read($@"{basePath}\payload-support-payment-maximum-attempts.json");

                case EEmailTemplate.SupportReversalMaximumAttempts:
                    return FileReaderHelper.Read($@"{basePath}\payload-support-reversal-maximum-attempts.json");

                case EEmailTemplate.SupportPaymentNotFoundForReversal:
                    return FileReaderHelper.Read($@"{basePath}\payload-support-payment-not-found-for-reversal.json");

                case EEmailTemplate.SupportPaymentAlreadyReversed:
                    return FileReaderHelper.Read($@"{basePath}\payload-payment-already-reversed.json");

                default:
                    return string.Empty;
            }
        }
    }
}