using Newtonsoft.Json;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.QueueLogs;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.QueueLogs.Queries.Results;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Domain.Emails.Commands.Inputs;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Common.Handlers
{
    public class BaseHandler
    {
        private static readonly string _emailQueue = QueueName.EmailNotifier;

        private readonly IRabbitMQBus _rabbitMQBus;
        private readonly IQueueLogRepository _queueLogRepository;
        private readonly IElmahRepository _elmahRepository;

        public BaseHandler(IRabbitMQBus rabbitMQBus,
                           IQueueLogRepository queueLogRepository,
                           IElmahRepository elmahRepository)
        {
            _rabbitMQBus = rabbitMQBus;
            _queueLogRepository = queueLogRepository;
            _elmahRepository = elmahRepository;
        }

        protected void SendToEmailQueue(PaymentCommand paymentCommand, EEmailTemplate emailTemplate, List<QueueLogQueryResult> queueLogs = null)
        {
            var emailNotification = new EmailNotificationCommand(paymentCommand, emailTemplate, queueLogs);
            _rabbitMQBus.Publish(emailNotification, _emailQueue);
        }

        protected async Task LogQueue(dynamic data, string applicationName, string currentQueue, string error = null)
        {
            var success = error == null;
            var message = JsonConvert.SerializeObject(data);
            var queueLog = new QueueLog(data.Id, applicationName, currentQueue, message, success, data.NumberAttempts, error);
            await _queueLogRepository.Log(queueLog);
        }

        protected async Task ControlMaximumAttempts(dynamic data, string applicationName, string currentQueue, EEmailTemplate emailTemplate, Exception exception)
        {
            await _elmahRepository.Log(exception);

            await LogQueue(data, applicationName, currentQueue, exception.Message);

            if (data.NumberAttempts < 3)
            {
                data.AddNumberAttempt();
                _rabbitMQBus.PublishDelayed(data, currentQueue);

                Console.WriteLine("An error occurred. Message has been registered again in the queue.");
            }
            else
            {
                var queueLogs = await _queueLogRepository.List(data.Id);

                SendToEmailQueue(data, emailTemplate, queueLogs);

                Console.WriteLine("An error occurred. Message exceeded the limit of three processing attempts. An email will be sent to support.");
            }
        }
    }
}