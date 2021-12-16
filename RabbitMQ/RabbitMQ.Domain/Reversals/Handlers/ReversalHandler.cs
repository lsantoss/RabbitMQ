using ElmahCore;
using Newtonsoft.Json;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.QueueLogs;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Domain.Emails.Commands.Inputs;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using RabbitMQ.Domain.Reversals.Commands.Inputs;
using RabbitMQ.Domain.Reversals.Interfaces.Handlers;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Reversals.Handlers
{
    public class ReversalHandler : IReversalHandler
    {
        private static readonly string _applicationName = ApplicationName.ConsumerReversals;
        private static readonly string _currentQueue = QueueName.Reversals;
        private static readonly string _nextQueue = QueueName.EmailNotifier;

        private readonly IRabbitMQBus _rabbitMQBus;
        private readonly IElmahRepository _elmahRepository;
        private readonly IQueueLogRepository _queueLogRepository;
        private readonly IPaymentRepository _paymentRepository;

        public ReversalHandler(IRabbitMQBus rabbitMQBus,
                              IElmahRepository elmahRepository,
                              IQueueLogRepository queueLogRepository,
                              IPaymentRepository paymentRepository)
        {
            _rabbitMQBus = rabbitMQBus;
            _elmahRepository = elmahRepository;
            _queueLogRepository = queueLogRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task Handle(PublishReversalCommand reversalCommand)
        {
            try
            {
                var paymentQueryResult = await _paymentRepository.Get(reversalCommand.Id);

                if (paymentQueryResult == null)
                {
                    //TODO: LogFila
                    //TODO: Publicar na fila de email, enviar email solicitando intervenção manual
                    return;
                }
                else if (paymentQueryResult.Reversed)
                {
                    //TODO: LogFila
                    //TODO: Publicar na fila de email, enviar email notificando que o estorno já havia sido realizado anteriormente
                    return;
                }

                var payment = paymentQueryResult.MapToPayment();
                payment.SetReversed(true);
                payment.SetChangeDate(DateTime.Now);
                await _paymentRepository.Update(payment);

                var message = JsonConvert.SerializeObject(reversalCommand);
                var queueLog = new QueueLog(payment.Id, _applicationName, _currentQueue, message);
                await _queueLogRepository.Log(queueLog);

                //TODO: Publicar na fila de email, enviar email notificando que o estorno foi efetuado com sucesso
                var emailNotification = new EmailNotificationCommand();
                _rabbitMQBus.Publish(emailNotification, _nextQueue);
            }
            catch (Exception ex)
            {
                await _elmahRepository.Log(new Error(ex));

                var message = JsonConvert.SerializeObject(reversalCommand);
                var queueLog = new QueueLog(reversalCommand.Id, _applicationName, _currentQueue, message, reversalCommand.NumberAttempts, ex.Message);
                await _queueLogRepository.Log(queueLog);

                if (reversalCommand.NumberAttempts < 3)
                {
                    reversalCommand.NumberAttempts++;
                    _rabbitMQBus.PublishDelayed(reversalCommand, _currentQueue);
                }
                else
                {
                    //TODO: Publicar na fila de email, enviar email solicitando intervenção manual
                    var emailNotification = new EmailNotificationCommand();
                    _rabbitMQBus.Publish(emailNotification, _nextQueue);
                }
            }
        }
    }
}