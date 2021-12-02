using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Domain.Core.Enums;

namespace RabbitMQ.Infra.Crosscutting
{
    public class WorkerBase
    {
        private IServiceCollection _services;
        private ServiceProvider _serviceProvider;
        private static IConfiguration configuration;

        public WorkerBase(EFila fila)
        {
            _services = new ServiceCollection();

            configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, false).AddEnvironmentVariables().Build();

            _services.AddSingleton(configuration);

            WorkerIoC.AddWorkerServices(_services, configuration, fila);

            _serviceProvider = _services.BuildServiceProvider();
        }

        public IServiceCollection GetServices() => _services;

        public IConfiguration GetConfiguration() => configuration;

        public T GetService<T>() => (T)_serviceProvider.GetService(typeof(T));
    }
}