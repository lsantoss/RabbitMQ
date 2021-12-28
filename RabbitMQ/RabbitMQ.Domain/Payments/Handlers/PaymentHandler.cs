using RabbitMQ.Domain.Common.Handlers;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Emails.Enums;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using RabbitMQ.Domain.Payments.Interfaces.Handlers;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Payments.Handlers
{
    public class PaymentHandler : BaseHandler, IPaymentHandler
    {
        private readonly string _applicationName;
        private readonly string _currentQueue;

        private readonly IPaymentRepository _paymentRepository;

        public PaymentHandler(IRabbitMQService rabbitMQBus,
                              IQueueLogRepository queueLogRepository,
                              IElmahRepository elmahRepository,
                              IPaymentRepository paymentRepository) : base(rabbitMQBus, queueLogRepository, elmahRepository)
        {
            _applicationName = ApplicationName.ConsumerPayments;
            _currentQueue = QueueName.Payments;

            _paymentRepository = paymentRepository;
        }

        public async Task HandleAsync(PaymentCommand paymentCommand)
        {
            Console.WriteLine("Handle started.");

            try
            {
                var payment = paymentCommand.MapToPayment();
                await _paymentRepository.SaveAsync(payment);

                await LogQueueAsync(paymentCommand, _applicationName, _currentQueue);

                SendToEmailQueue(paymentCommand.PaymentId, EEmailTemplate.PaymentSuccess);

                Console.WriteLine("Payment registered successfully.");
            }
            catch (Exception ex)
            {
                await ControlMaximumAttemptsAsync(paymentCommand, _applicationName, _currentQueue, EEmailTemplate.SupportPaymentMaximumAttempts, ex);
            }

            Console.WriteLine("Handle finished.\n");
        }
    }
}