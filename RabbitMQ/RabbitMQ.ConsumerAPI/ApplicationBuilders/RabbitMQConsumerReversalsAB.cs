using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Reversals.Commands.Inputs;
using RabbitMQ.Domain.Reversals.Interfaces.Handlers;

namespace RabbitMQ.ConsumerAPI.ApplicationBuilders
{
    public static class RabbitMQConsumerReversalsAB
    {
        private static IApplicationBuilder _applicationBuilder;
        private static IServiceScope _scope;

        public static IApplicationBuilder UseRabbitConsumerReversals(this IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
            var life = _applicationBuilder.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            _ = life.ApplicationStarted.Register(StarConsumerReversals);
            return _applicationBuilder;
        }

        private static void StarConsumerReversals()
        {
            using (_scope = _applicationBuilder.ApplicationServices.CreateScope())
            {
                var service = _scope.ServiceProvider.GetRequiredService<IRabbitMQService>();
                var handler = _scope.ServiceProvider.GetRequiredService<IReversalHandler>();
                service.Consume<ReversalCommand>(handler, QueueName.Reversal);
            }
        }
    }
}