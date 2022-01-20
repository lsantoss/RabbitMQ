using RabbitMQ.Domain.Common.Handlers;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using RabbitMQ.Domain.Reversals.Commands.Inputs;
using RabbitMQ.Domain.Reversals.Entities;
using RabbitMQ.Domain.Reversals.Interfaces.Handlers;
using RabbitMQ.Domain.Reversals.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Reversals.Handlers
{
    public class ReversalHandler : BaseHandler, IReversalHandler
    {
        private readonly string _currentQueue = QueueName.Reversals;
        private readonly string _applicationName = AppDomain.CurrentDomain.FriendlyName;

        private readonly IQueueLogRepository _queueLogRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IReversalRepository _reversalRepository;

        public ReversalHandler(IRabbitMQService rabbitMQBus,
                               IQueueLogRepository queueLogRepository,
                               IElmahRepository elmahRepository,
                               IPaymentRepository paymentRepository,
                               IReversalRepository reversalRepository) : base(rabbitMQBus, queueLogRepository, elmahRepository)
        {
            _queueLogRepository = queueLogRepository;
            _paymentRepository = paymentRepository;
            _reversalRepository = reversalRepository;
        }

        public async Task HandleAsync(ReversalCommand reversalCommand)
        {
            Console.WriteLine("Handle started.");

            try
            {
                var paymentQueryResult = await _paymentRepository.GetAsync(reversalCommand.PaymentId);

                if (paymentQueryResult == null)
                {
                    await LogQueueAsync(reversalCommand, _applicationName, _currentQueue, "Payment not found in the database");

                    SendToEmailQueue(reversalCommand.PaymentId, EEmailTemplate.SupportPaymentNotFoundForReversal);

                    Console.WriteLine("This payment is not registered in our database. An email will be sent to support.");
                }                
                else if (paymentQueryResult.Reversed)
                {
                    await LogQueueAsync(reversalCommand, _applicationName, _currentQueue, "Payment has already been reversed");

                    var queueLogs = await _queueLogRepository.ListAsync(reversalCommand.PaymentId);

                    SendToEmailQueue(reversalCommand.PaymentId, EEmailTemplate.SupportPaymentAlreadyReversed, queueLogs);

                    Console.WriteLine("This payment has already been reversed. An email will be sent to support.");
                }
                else
                {
                    var payment = paymentQueryResult.MapToPayment();
                    payment.Reverse();
                    await _paymentRepository.UpdateAsync(payment);

                    var reversal = new Reversal(reversalCommand.PaymentId, reversalCommand.Date);
                    await _reversalRepository.SaveAsync(reversal);

                    await LogQueueAsync(reversalCommand, _applicationName, _currentQueue);
                    
                    if (payment.NotifyByEmail)
                        SendToEmailQueue(reversalCommand.PaymentId, EEmailTemplate.ReversalSuccess);

                    Console.WriteLine("Reversal registered successfully.");
                }                
            }
            catch (Exception ex)
            {
                await ControlMaximumAttemptsAsync(reversalCommand, _applicationName, _currentQueue, EEmailTemplate.SupportReversalMaximumAttempts, ex);
            }

            Console.WriteLine("Handle finished.\n");
        }
    }
}