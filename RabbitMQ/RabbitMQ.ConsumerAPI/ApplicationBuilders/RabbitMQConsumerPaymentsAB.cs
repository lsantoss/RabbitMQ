using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using RabbitMQ.Domain.Payments.Interfaces.Handlers;

namespace RabbitMQ.ConsumerAPI.ApplicationBuilders
{
    public static class RabbitMQConsumerPaymentsAB
    {
        private static IApplicationBuilder _applicationBuilder;
        private static IServiceScope _scope;

        public static IApplicationBuilder UseRabbitConsumerPayments(this IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
            var life = _applicationBuilder.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            _ = life.ApplicationStarted.Register(StarConsumerPayments);
            return _applicationBuilder;
        }

        private static void StarConsumerPayments()
        {
            using (_scope = _applicationBuilder.ApplicationServices.CreateScope())
            {
                var service = _scope.ServiceProvider.GetRequiredService<IRabbitMQService>();
                var paymentHandler = _scope.ServiceProvider.GetRequiredService<IPaymentHandler>();
                service.Consume<PaymentCommand>(paymentHandler, QueueName.Payment);
            }
        }
    }
}