using ElmahCore;

namespace RabbitMQ.Domain.Core.Elmah.Interfaces
{
    public interface IElmahRepository
    {
        string Log(Error error);
    }
}