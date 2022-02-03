using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Reversals.Commands.Inputs;
using RabbitMQ.Domain.Reversals.Interfaces.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.ConsumerAPI.BackgroundServices
{
    public class RabbitMQConsumerReversals : BackgroundService
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IReversalHandler _reversalHandler;

        public RabbitMQConsumerReversals(IServiceScopeFactory factory)
        {
            var serviceProvider = factory.CreateScope().ServiceProvider;
            _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            _reversalHandler = serviceProvider.GetRequiredService<IReversalHandler>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Starting BackgroundService {GetType().Name} in {AppDomain.CurrentDomain.FriendlyName}\n");

            stoppingToken.Register(() => Console.WriteLine($"Stopping BackgroundService {GetType().Name} in {AppDomain.CurrentDomain.FriendlyName}.\n"));

            while (!stoppingToken.IsCancellationRequested)
                _rabbitMQService.Consume<ReversalCommand>(_reversalHandler, QueueName.Reversal);
        }
    }
}