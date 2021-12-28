using Newtonsoft.Json;
using RabbitMQ.Domain.Common.Commands.Inputs;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.QueueLogs;
using RabbitMQ.Domain.Core.QueueLogs.Entities;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.QueueLogs.Queries.Results;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Emails.Commands.Inputs;
using RabbitMQ.Domain.Emails.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Common.Handlers
{
    public class BaseHandler
    {
        private static readonly string _emailQueue = QueueName.EmailNotifier;

        private readonly IRabbitMQService _rabbitMQBus;
        private readonly IQueueLogRepository _queueLogRepository;
        private readonly IElmahRepository _elmahRepository;

        public BaseHandler(IRabbitMQService rabbitMQBus,
                           IQueueLogRepository queueLogRepository,
                           IElmahRepository elmahRepository)
        {
            _rabbitMQBus = rabbitMQBus;
            _queueLogRepository = queueLogRepository;
            _elmahRepository = elmahRepository;
        }

        protected void SendToEmailQueue(Guid paymentId, EEmailTemplate emailTemplate, List<QueueLogQueryResult> queueLogs = null)
        {
            var emailCommand = new EmailCommand(paymentId, emailTemplate, queueLogs);
            _rabbitMQBus.Publish(emailCommand, _emailQueue);
        }

        protected async Task LogQueue(Command command, string applicationName, string currentQueue, string error = null)
        {
            var success = error == null;
            var message = JsonConvert.SerializeObject(command);
            var queueLog = new QueueLog(command.PaymentId, applicationName, currentQueue, message, success, command.NumberAttempts, error);
            await _queueLogRepository.Log(queueLog);
        }

        protected async Task ControlMaximumAttempts(Command command, string applicationName, string currentQueue, EEmailTemplate emailTemplate, Exception exception)
        {
            await _elmahRepository.Log(exception);

            await LogQueue(command, applicationName, currentQueue, exception.Message);

            if (command.NumberAttempts < 3)
            {
                command.AddNumberAttempt();
                _rabbitMQBus.PublishDelayed(command, currentQueue);

                Console.WriteLine("An error occurred. Message has been registered again in the queue.");
            }
            else
            {
                var queueLogs = await _queueLogRepository.List(command.PaymentId);

                SendToEmailQueue(command.PaymentId, emailTemplate, queueLogs);

                Console.WriteLine("An error occurred. Message exceeded the limit of three processing attempts. An email will be sent to support.");
            }
        }
    }
}