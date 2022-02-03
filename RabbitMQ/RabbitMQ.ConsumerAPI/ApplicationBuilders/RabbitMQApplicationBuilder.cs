using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Domain.Core.Constants;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Emails.Commands.Inputs;
using RabbitMQ.Domain.Emails.Interfaces.Handlers;
using RabbitMQ.Domain.Payments.Commands.Inputs;
using RabbitMQ.Domain.Payments.Interfaces.Handlers;
using RabbitMQ.Domain.Reversals.Commands.Inputs;
using RabbitMQ.Domain.Reversals.Interfaces.Handlers;

namespace RabbitMQ.ConsumerAPI.ApplicationBuilders
{
    public static class RabbitMQAplicationBuilder
    {
        private static IApplicationBuilder _applicationBuilder;
        private static IServiceScope _scope;

        public static IApplicationBuilder UseRabbitConsumerPayment(this IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
            var life = _applicationBuilder.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            _ = life.ApplicationStarted.Register(StarConsumerPayment);
            return _applicationBuilder;
        }

        public static IApplicationBuilder UseRabbitConsumerReversal(this IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
            var life = _applicationBuilder.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            _ = life.ApplicationStarted.Register(StarConsumerReversal);
            return _applicationBuilder;
        }

        public static IApplicationBuilder UseRabbitConsumerEmail(this IApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
            var life = _applicationBuilder.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            _ = life.ApplicationStarted.Register(StarConsumerEmail);
            return _applicationBuilder;
        }

        private static void StarConsumerPayment()
        {
            using (_scope = _applicationBuilder.ApplicationServices.CreateScope())
            {
                var service = _scope.ServiceProvider.GetRequiredService<IRabbitMQService>();
                var paymentHandler = _scope.ServiceProvider.GetRequiredService<IPaymentHandler>();
                service.Consume<PaymentCommand>(paymentHandler, QueueName.Payment);
            }
        }

        private static void StarConsumerReversal()
        {
            using (_scope = _applicationBuilder.ApplicationServices.CreateScope())
            {
                var service = _scope.ServiceProvider.GetRequiredService<IRabbitMQService>();
                var handler = _scope.ServiceProvider.GetRequiredService<IReversalHandler>();
                service.Consume<ReversalCommand>(handler, QueueName.Reversal);
            }
        }

        private static void StarConsumerEmail()
        {
            using (_scope = _applicationBuilder.ApplicationServices.CreateScope())
            {
                var service = _scope.ServiceProvider.GetRequiredService<IRabbitMQService>();
                var handler = _scope.ServiceProvider.GetRequiredService<IEmailHandler>();
                service.Consume<EmailCommand>(handler, QueueName.Email);
            }
        }
    }
}