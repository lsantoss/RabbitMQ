using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Domain.Core.Enums;

namespace RabbitMQ.Infra.Crosscutting
{
    public class WorkerBase : IWorkerBase
    {
        private readonly IServiceCollection _services;
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider _serviceProvider;

        public WorkerBase(EApplication application)
        {
            _services = new ServiceCollection();

            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, false).AddEnvironmentVariables().Build();

            _services.AddSingleton(_configuration);

            WorkerIoC.AddWorkerServices(_services, _configuration, application);

            _serviceProvider = _services.BuildServiceProvider();
        }

        public IConfiguration GetConfiguration() => _configuration;

        public IServiceCollection GetServices() => _services;

        public T GetService<T>() => (T)_serviceProvider.GetService(typeof(T));
    }
}