using Newtonsoft.Json;
using RabbitMQ.Domain.Common.Commands.Inputs;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
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
        private static readonly string _emailQueue = QueueName.Email;

        private readonly IRabbitMQService _rabbitMQService;
        private readonly IQueueLogRepository _queueLogRepository;
        private readonly IElmahRepository _elmahRepository;

        public BaseHandler(IRabbitMQService rabbitMQBus,
                           IQueueLogRepository queueLogRepository,
                           IElmahRepository elmahRepository)
        {
            _rabbitMQService = rabbitMQBus;
            _queueLogRepository = queueLogRepository;
            _elmahRepository = elmahRepository;
        }

        protected void SendToEmailQueue(Guid paymentId, EEmailTemplate emailTemplate, List<QueueLogQueryResult> queueLogs = null)
        {
            var emailCommand = new EmailCommand(paymentId, emailTemplate, queueLogs);
            _rabbitMQService.Publish(emailCommand, _emailQueue);
        }

        protected async Task LogQueueAsync(Command command, string applicationName, string currentQueue, string error = null)
        {
            var success = error == null;
            var message = JsonConvert.SerializeObject(command);
            var queueLog = new QueueLog(command.PaymentId, applicationName, currentQueue, message, success, command.NumberAttempts, error);
            _ = await _queueLogRepository.LogAsync(queueLog);
        }

        protected async Task ControlMaximumAttemptsAsync(Command command, string applicationName, string currentQueue, EEmailTemplate emailTemplate, Exception exception)
        {
            _ = await _elmahRepository.LogAsync(exception);

            await LogQueueAsync(command, applicationName, currentQueue, exception.Message);

            if (command.NumberAttempts < 3)
            {
                command.AddNumberAttempt();

                _rabbitMQService.PublishDelayed(command, currentQueue);

                Console.WriteLine("An error occurred. Message has been registered again in the queue.");
            }
            else
            {
                var queueLogs = await _queueLogRepository.ListAsync(command.PaymentId);

                SendToEmailQueue(command.PaymentId, emailTemplate, queueLogs);

                Console.WriteLine("An error occurred. Message exceeded the limit of three processing attempts. An email will be sent to support.");
            }
        }

        protected async Task ControlMaximumAttemptsAsync(NotificationCommand command, string applicationName, string currentQueue, EEmailTemplate emailTemplate, Exception exception)
        {
            _ = await _elmahRepository.LogAsync(exception);

            await LogQueueAsync(command, applicationName, currentQueue, exception.Message);

            if (command.NumberAttempts < 3)
            {
                command.AddNumberAttempt();

                _rabbitMQService.PublishDelayed(command, currentQueue);

                Console.WriteLine("An error occurred. Message has been registered again in the queue.");
            }
            else
            {
                Console.WriteLine("An error occurred. Message exceeded the limit of three processing attempts. Notification sending failed.");
            }
        }
    }
}