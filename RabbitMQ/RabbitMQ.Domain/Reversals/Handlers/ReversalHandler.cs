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
            Console.WriteLine("Handle started.");

            try
            {
                var paymentQueryResult = await _paymentRepository.Get(reversalCommand.Id);

                if (paymentQueryResult == null)
                {
                    var message = JsonConvert.SerializeObject(reversalCommand);
                    var queueLog = new QueueLog(reversalCommand.Id, _applicationName, _currentQueue, message, reversalCommand.NumberAttempts, "Pagamento não encontrado na base de dados");
                    await _queueLogRepository.Log(queueLog);

                    //TODO: Publicar na fila de email, enviar email solicitando intervenção manual

                    Console.WriteLine("An error has occurred. This payment is not registered in our database. An email will be sent to support.");
                }                
                else if (paymentQueryResult.Reversed)
                {
                    var message = JsonConvert.SerializeObject(reversalCommand);
                    var queueLog = new QueueLog(reversalCommand.Id, _applicationName, _currentQueue, message, reversalCommand.NumberAttempts, "Pagamento já foi estornado anteriormente");
                    await _queueLogRepository.Log(queueLog);

                    //TODO: Publicar na fila de email, enviar email notificando que o estorno já havia sido realizado anteriormente

                    Console.WriteLine("An error has occurred. This payment has already been reversed.");
                }
                else
                {
                    var payment = paymentQueryResult.MapToPayment();
                    payment.Reverse();
                    await _paymentRepository.Update(payment);

                    var message = JsonConvert.SerializeObject(reversalCommand);
                    var queueLog = new QueueLog(payment.Id, _applicationName, _currentQueue, message);
                    await _queueLogRepository.Log(queueLog);

                    //TODO: Publicar na fila de email, enviar email notificando que o estorno foi efetuado com sucesso

                    Console.WriteLine("Reversal registered successfully.");
                }
                
            }
            catch (Exception ex)
            {
                await _elmahRepository.Log(new Error(ex));

                var message = JsonConvert.SerializeObject(reversalCommand);
                var queueLog = new QueueLog(reversalCommand.Id, _applicationName, _currentQueue, message, reversalCommand.NumberAttempts, ex.Message);
                await _queueLogRepository.Log(queueLog);

                if (reversalCommand.NumberAttempts < 3)
                {
                    reversalCommand.AddNumberAttempt();
                    _rabbitMQBus.PublishDelayed(reversalCommand, _currentQueue);

                    Console.WriteLine("An error occurred. Reversal has been registered again in the queue.");
                }
                else
                {
                    //TODO: Publicar na fila de email, enviar email solicitando intervenção manual

                    Console.WriteLine("An error occurred. Reversal exceeded the limit of three processing attempts. An email will be sent to support.");
                }
            }

            Console.WriteLine("Handle finished.\n");
        }
    }
}