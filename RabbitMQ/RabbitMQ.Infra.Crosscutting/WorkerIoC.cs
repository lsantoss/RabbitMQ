using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Domain.Common.Handlers;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.Emails.Interfaces.Services;
using RabbitMQ.Domain.Core.Emails.Services;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using RabbitMQ.Domain.Core.RabbitMQ.Services;
using RabbitMQ.Domain.Payments.Handlers;
using RabbitMQ.Domain.Payments.Interfaces.Handlers;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using RabbitMQ.Domain.Reversals.Handlers;
using RabbitMQ.Domain.Reversals.Interfaces.Handlers;
using RabbitMQ.Infra.Data.Repositories;

namespace RabbitMQ.Infra.Crosscutting
{
    public static class WorkerIoC
    {
        public static IServiceCollection AddWorkerServices(this IServiceCollection services, IConfiguration configuration, EApplication application)
        {
            #region AppSettings

            var settings = new Settings();
            configuration.GetSection("Settings").Bind(settings);
            services.AddSingleton(settings);

            var rabbitMQSettings = new RabbitMQSettings();
            configuration.GetSection("RabbitMQSettings").Bind(rabbitMQSettings);
            services.AddSingleton(rabbitMQSettings);

            var smtpSettings = new SmtpSettings();
            configuration.GetSection("SmtpSettings").Bind(smtpSettings);
            services.AddSingleton(smtpSettings);

            #endregion AppSettings

            #region Repositories

            services.AddScoped<IElmahRepository, ElmahRepository>();
            services.AddScoped<IQueueLogRepository, QueueLogRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            #endregion Repositories

            #region Handlers

            services.AddScoped<BaseHandler, BaseHandler>();

            if (application == EApplication.ConsumerPayments)
                services.AddScoped<IPaymentHandler, PaymentHandler>();

            if (application == EApplication.ConsumerReversals)
                services.AddScoped<IReversalHandler, ReversalHandler>();            

            #endregion Handlers

            #region Services

            services.AddScoped<IRabbitMQService, RabbitMQService>();
            services.AddScoped<IEmailService, EmailService>();

            #endregion Services

            return services;
        }
    }
}