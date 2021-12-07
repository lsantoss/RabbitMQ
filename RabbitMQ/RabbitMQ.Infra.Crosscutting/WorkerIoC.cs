using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Infra.Data.Repositories;

namespace RabbitMQ.Infra.Crosscutting
{
    public static class WorkerIoC
    {
        public static IServiceCollection AddWorkerServices(this IServiceCollection services, IConfiguration configuration, EQueue queue)
        {
            #region AppSettings

            var settings = new Settings();
            configuration.GetSection("Settings").Bind(settings);
            services.AddSingleton(settings);

            var queuesWorkersSettings = new QueuesWorkersSettings();
            configuration.GetSection("QueuesWorkersSettings").Bind(queuesWorkersSettings);
            services.AddSingleton(queuesWorkersSettings);

            var rabbitMQSettings = new RabbitMQSettings();
            configuration.GetSection("RabbitMQSettings").Bind(rabbitMQSettings);
            services.AddSingleton(rabbitMQSettings);

            #endregion AppSettings

            #region Repositories

            services.AddScoped<IElmahRepository, ElmahRepository>();
            services.AddScoped<IQueueLogRepository, QueueLogRepository>();

            #endregion Repositories

            #region Handlers

            if (queue == EQueue.ConsumerPayments)
                //services.AddScoped<IConsumerPaymentHandler, ConsumerPaymentHandler>();

            #endregion Handlers

            #region MemoryCache

            services.AddMemoryCache();

            #endregion MemoryCache

            #region RabbitMQ

            services.AddScoped<IRabbitMQBus, RabbitMQBus>();

            #endregion RabbitMQ

            return services;
        }
    }
}