using RabbitMQ.Domain.Emails.Commands.Inputs;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Emails.Interfaces.Handlers
{
    public interface IEmailHandler
    {
        Task HandleAsync(EmailCommand emailCommand);
    }
}