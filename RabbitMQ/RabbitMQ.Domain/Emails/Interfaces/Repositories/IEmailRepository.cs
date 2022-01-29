using RabbitMQ.Domain.Emails.Entities;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Emails.Interfaces.Repositories
{
    public interface IEmailRepository
    {
        Task SaveAsync(Email email);
    }
}