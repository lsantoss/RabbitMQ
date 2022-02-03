using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Domain.Common.Handlers;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.Emails.Interfaces.Services;
using RabbitMQ.Domain.Core.Emails.Services;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Core.RabbitMQ.Services;
using RabbitMQ.Domain.Emails.Handlers;
using RabbitMQ.Domain.Emails.Interfaces.Handlers;
using RabbitMQ.Domain.Emails.Interfaces.Repositories;
using RabbitMQ.Domain.Payments.Handlers;
using RabbitMQ.Domain.Payments.Interfaces.Handlers;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using RabbitMQ.Domain.Reversals.Handlers;
using RabbitMQ.Domain.Reversals.Interfaces.Handlers;
using RabbitMQ.Domain.Reversals.Interfaces.Repositories;
using RabbitMQ.Infra.Data.Repositories;

namespace RabbitMQ.Infra.Crosscutting
{
    public static class IoC
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            _ = services.AddAppSettings(configuration);
            _ = services.AddRepositories();
            _ = services.AddHandlers();
            _ = services.AddServices();
            return services;
        }

        private static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new Settings();
            configuration.GetSection("Settings").Bind(settings);
            _ = services.AddSingleton(settings);

            var rabbitMQSettings = new RabbitMQSettings();
            configuration.GetSection("RabbitMQSettings").Bind(rabbitMQSettings);
            _ = services.AddSingleton(rabbitMQSettings);

            var smtpSettings = new SmtpSettings();
            configuration.GetSection("SmtpSettings").Bind(smtpSettings);
            _ = services.AddSingleton(smtpSettings);

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            _ = services.AddScoped<IElmahRepository, ElmahRepository>();
            _ = services.AddScoped<IQueueLogRepository, QueueLogRepository>();
            _ = services.AddScoped<IPaymentRepository, PaymentRepository>();
            _ = services.AddScoped<IReversalRepository, ReversalRepository>();
            _ = services.AddScoped<IEmailRepository, EmailRepository>();
            return services;
        }

        private static IServiceCollection AddHandlers(this IServiceCollection services)
        {
            _ = services.AddScoped<BaseHandler, BaseHandler>();
            _ = services.AddScoped<IPaymentHandler, PaymentHandler>();
            _ = services.AddScoped<IReversalHandler, ReversalHandler>();
            _ = services.AddScoped<IEmailHandler, EmailHandler>();
            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            _ = services.AddScoped<IRabbitMQService, RabbitMQService>();
            _ = services.AddScoped<IEmailSenderService, EmailSenderService>();
            return services;
        }
    }
}