﻿using Microsoft.Extensions.Configuration;
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
using RabbitMQ.Domain.Emails.Handlers;
using RabbitMQ.Domain.Emails.Interfaces.Handlers;
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
            _ = services.AddSingleton(settings);

            var rabbitMQSettings = new RabbitMQSettings();
            configuration.GetSection("RabbitMQSettings").Bind(rabbitMQSettings);
            _ = services.AddSingleton(rabbitMQSettings);

            var smtpSettings = new SmtpSettings();
            configuration.GetSection("SmtpSettings").Bind(smtpSettings);
            _ = services.AddSingleton(smtpSettings);

            #endregion AppSettings

            #region Repositories

            _ = services.AddScoped<IElmahRepository, ElmahRepository>();
            _ = services.AddScoped<IQueueLogRepository, QueueLogRepository>();
            _ = services.AddScoped<IPaymentRepository, PaymentRepository>();

            #endregion Repositories

            #region Handlers

            _ = services.AddScoped<BaseHandler, BaseHandler>();

            switch (application)
            {
                case EApplication.ConsumerPayments:
                    _ = services.AddScoped<IPaymentHandler, PaymentHandler>();
                    break;

                case EApplication.ConsumerReversals:
                    _ = services.AddScoped<IReversalHandler, ReversalHandler>();
                    break;

                case EApplication.EmailNotifier:
                    _ = services.AddScoped<IEmailHandler, EmailHandler>();
                    break;
            }

            #endregion Handlers

            #region Services

            _ = services.AddScoped<IRabbitMQService, RabbitMQService>();
            _ = services.AddScoped<IEmailSenderService, EmailSenderService>();

            #endregion Services

            return services;
        }
    }
}