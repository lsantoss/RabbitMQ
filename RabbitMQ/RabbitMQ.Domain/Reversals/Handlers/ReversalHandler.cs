using RabbitMQ.Domain.Common.Handlers;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using RabbitMQ.Domain.Reversals.Commands.Inputs;
using RabbitMQ.Domain.Reversals.Interfaces.Handlers;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Reversals.Handlers
{
    public class ReversalHandler : BaseHandler, IReversalHandler
    {
        private static readonly string _applicationName = ApplicationName.ConsumerReversals;
        private static readonly string _currentQueue = QueueName.Reversals;

        private readonly IPaymentRepository _paymentRepository;

        public ReversalHandler(IRabbitMQBus rabbitMQBus,
                               IQueueLogRepository queueLogRepository,
                               IElmahRepository elmahRepository,
                               IPaymentRepository paymentRepository) : base(rabbitMQBus, queueLogRepository, elmahRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task Handle(ReversalCommand reversalCommand)
        {
            Console.WriteLine("Handle started.");

            try
            {
                var paymentQueryResult = await _paymentRepository.Get(reversalCommand.Id);

                if (paymentQueryResult == null)
                {
                    await LogQueue(reversalCommand, _applicationName, _currentQueue, "Pagamento não encontrado na base de dados");

                    //TODO: Publicar na fila de email, enviar email solicitando intervenção manual

                    Console.WriteLine("An error has occurred. This payment is not registered in our database. An email will be sent to support.");
                }                
                else if (paymentQueryResult.Reversed)
                {
                    await LogQueue(reversalCommand, _applicationName, _currentQueue, "Pagamento já foi estornado anteriormente");

                    //TODO: Publicar na fila de email, enviar email notificando que o estorno já havia sido realizado anteriormente

                    Console.WriteLine("An error has occurred. This payment has already been reversed.");
                }
                else
                {
                    var payment = paymentQueryResult.MapToPayment();
                    payment.Reverse();
                    await _paymentRepository.Update(payment);

                    await LogQueue(reversalCommand, _applicationName, _currentQueue);

                    //TODO: Publicar na fila de email, enviar email notificando que o estorno foi efetuado com sucesso

                    Console.WriteLine("Reversal registered successfully.");
                }                
            }
            catch (Exception ex)
            {
                await ControlMaximumAttempts(reversalCommand, _applicationName, _currentQueue, EEmailTemplate.SupportReversalMaximumAttempts, ex);
            }

            Console.WriteLine("Handle finished.\n");
        }
    }
}