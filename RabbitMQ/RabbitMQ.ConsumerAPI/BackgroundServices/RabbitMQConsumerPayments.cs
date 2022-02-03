using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using RabbitMQ.Domain.Payments.Interfaces.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.ConsumerAPI.BackgroundServices
{
    public class RabbitMQConsumerPayments : BackgroundService
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IPaymentHandler _paymentHandler;

        public RabbitMQConsumerPayments(IServiceScopeFactory factory)
        {
            var serviceProvider = factory.CreateScope().ServiceProvider;
            _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            _paymentHandler = serviceProvider.GetRequiredService<IPaymentHandler>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Starting BackgroundService {GetType().Name} in {AppDomain.CurrentDomain.FriendlyName}\n");

            stoppingToken.Register(() => Console.WriteLine($"Stopping BackgroundService {GetType().Name} in {AppDomain.CurrentDomain.FriendlyName}.\n"));

            while (!stoppingToken.IsCancellationRequested)
                _rabbitMQService.Consume<PaymentCommand>(_paymentHandler, QueueName.Payment);
        }
    }
}