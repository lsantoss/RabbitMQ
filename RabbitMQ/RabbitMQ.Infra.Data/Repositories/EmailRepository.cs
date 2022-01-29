using Dapper;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Emails.Entities;
using RabbitMQ.Domain.Emails.Interfaces.Repositories;
using RabbitMQ.Infra.Data.Repositories.Queries;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RabbitMQ.Infra.Data.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly Settings _settings;
        private readonly DynamicParameters _parameters = new();

        public EmailRepository(Settings settings)
        {
            _settings = settings;
        }

        public async Task SaveAsync(Email email)
        {
            _parameters.Add("Id", email.Id, DbType.Guid);
            _parameters.Add("PaymentId", email.PaymentId, DbType.Guid);
            _parameters.Add("Notified", email.Notified, DbType.Boolean);
            _parameters.Add("CreationDate", email.CreationDate, DbType.DateTime);

            using var connection = new SqlConnection(_settings.ConnectionString);

            await connection.ExecuteAsync(EmailQueries.Save, _parameters);
        }
    }
}
