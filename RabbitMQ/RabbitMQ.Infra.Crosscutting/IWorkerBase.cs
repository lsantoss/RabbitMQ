using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RabbitMQ.Infra.Crosscutting
{
    public interface IWorkerBase
    {
        IConfiguration GetConfiguration();
        IServiceCollection GetServices();
        T GetService<T>();
    }
}