using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.RabbitMQ;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Infra.Data.Repositories;

namespace RabbitMQ.Infra.Crosscutting
{
    public static class WorkerIoC
    {
        public static IServiceCollection AddWorkerServices(this IServiceCollection services, IConfiguration configuration, EFila worker)
        {
            #region AppSettings

            var settings = new Settings();
            configuration.GetSection("Settings").Bind(settings);
            services.AddSingleton(settings);

            var filasWorkersSettings = new FilasWorkersSettings();
            configuration.GetSection("FilasWorkersSettings").Bind(filasWorkersSettings);
            services.AddSingleton(filasWorkersSettings);

            var rabbitMQSettings = new RabbitMQSettings();
            configuration.GetSection("RabbitMQSettings").Bind(rabbitMQSettings);
            services.AddSingleton(rabbitMQSettings);

            #endregion AppSettings

            #region Repositories

            services.AddTransient<IElmahRepository, ElmahRepository>();
            //services.AddScoped<ILogFilaRepository, LogFilaRepository>();

            #endregion Repositories

            #region Handlers

            if (worker == EFila.ConsumirPagamento)
                //services.AddScoped<IConsumirPagamentoHandler, ConsumirPagamentoHandler>();

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