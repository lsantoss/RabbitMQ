using System;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Core.Elmah.Interfaces.Repository
{
    public interface IElmahRepository
    {
        Task<string> Log(Exception exception);
    }
}