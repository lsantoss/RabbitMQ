using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Domain.Core.Enums;

namespace RabbitMQ.Infra.Crosscutting
{
    public class WorkerBase
    {
        private readonly IServiceCollection _services;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider _serviceProvider;

        public WorkerBase(EQueue fila)
        {
            _services = new ServiceCollection();

            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, false).AddEnvironmentVariables().Build();

            _services.AddSingleton(_configuration);

            WorkerIoC.AddWorkerServices(_services, _configuration, fila);

            _serviceProvider = _services.BuildServiceProvider();
        }

        public IServiceCollection GetServices() => _services;

        public IConfiguration GetConfiguration() => _configuration;

        public T GetService<T>() => (T)_serviceProvider.GetService(typeof(T));
    }
}