using ElmahCore;
using Newtonsoft.Json;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.QueueLogs;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Domain.Emails.Commands.Inputs;
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

                //TODO: Publicar na fila de email, enviar email notificando que o pagamento foi efetuado com sucesso
                var emailNotification = new EmailNotificationCommand();
                _rabbitMQBus.Publish(emailNotification, _nextQueue);
            }
            catch (Exception ex)
            {
                var message = JsonConvert.SerializeObject(paymentCommand);
                var queueLog = new QueueLog(paymentCommand.Id, _applicationName, _currentQueue, message, paymentCommand.NumberAttempts, ex.Message);

                await _queueLogRepository.Log(queueLog);
                await _elmahRepository.Log(new Error(ex));

                if (paymentCommand.NumberAttempts < 3)
                {
                    paymentCommand.NumberAttempts++;
                    _rabbitMQBus.PublishDelayed(paymentCommand, _currentQueue);
                }
                else
                {
                    //TODO: Publicar na fila de email, enviar email solicitando intervenção manual
                }
            }
        }
    }
}