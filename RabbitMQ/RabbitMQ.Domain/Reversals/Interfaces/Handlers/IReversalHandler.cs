using RabbitMQ.Domain.Reversals.Commands.Inputs;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Reversals.Interfaces.Handlers
{
    public interface IReversalHandler
    {
        Task HandleAsync(ReversalCommand reversalCommand);
    }
}