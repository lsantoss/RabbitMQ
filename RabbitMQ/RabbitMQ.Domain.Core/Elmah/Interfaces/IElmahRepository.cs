using ElmahCore;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Core.Elmah.Interfaces
{
    public interface IElmahRepository
    {
        Task<string> Log(Error error);
    }
}