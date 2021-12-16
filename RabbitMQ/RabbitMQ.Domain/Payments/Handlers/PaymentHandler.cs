using ElmahCore;
using Newtonsoft.Json;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.QueueLogs;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Domain.Emails.Commands.Inputs;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using RabbitMQ.Domain.Payments.Interfaces.Handlers;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Payments.Handlers
{
    public class PaymentHandler : IPaymentHandler
    {
        private static readonly string _applicationName = ApplicationName.ConsumerPayments;
        private static readonly string _currentQueue = QueueName.Payments;
        private static readonly string _nextQueue = QueueName.EmailNotifier;

        private readonly IRabbitMQBus _rabbitMQBus;
        private readonly IElmahRepository _elmahRepository;
        private readonly IQueueLogRepository _queueLogRepository;
        private readonly IPaymentRepository _paymentRepository;

        public PaymentHandler(IRabbitMQBus rabbitMQBus,
                              IElmahRepository elmahRepository,
                              IQueueLogRepository queueLogRepository,
                              IPaymentRepository paymentRepository)
        {
            _rabbitMQBus = rabbitMQBus;
            _elmahRepository = elmahRepository;
            _queueLogRepository = queueLogRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task Handle(PublishPaymentCommand paymentCommand)
        {         
            try
            {
                var payment = paymentCommand.MapToPayment();
                await _paymentRepository.Save(payment);

                var message = JsonConvert.SerializeObject(paymentCommand);
                var queueLog = new QueueLog(payment.Id, _applicationName, _currentQueue, message);
                await _queueLogRepository.Log(queueLog);

                var emailNotification = new EmailNotificationCommand(payment, EEmailTemplate.PaymentSuccess);
                _rabbitMQBus.Publish(emailNotification, _nextQueue);
            }
            catch (Exception ex)
            {
                await _elmahRepository.Log(new Error(ex));

                var message = JsonConvert.SerializeObject(paymentCommand);
                var queueLog = new QueueLog(paymentCommand.Id, _applicationName, _currentQueue, message, paymentCommand.NumberAttempts, ex.Message);
                await _queueLogRepository.Log(queueLog);

                if (paymentCommand.NumberAttempts < 3)
                {
                    paymentCommand.NumberAttempts++;
                    _rabbitMQBus.PublishDelayed(paymentCommand, _currentQueue);
                }
                else
                {
                    var queueLogsQueryResult = await _queueLogRepository.List(paymentCommand.Id);
                    var emailNotification = new EmailNotificationCommand(paymentCommand, EEmailTemplate.SupportPaymentMaximumAttempts, queueLogsQueryResult);
                    _rabbitMQBus.Publish(emailNotification, _nextQueue);
                }
            }
        }
    }
}