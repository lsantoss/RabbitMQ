using Dapper;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Payments.Entities;
using RabbitMQ.Domain.Payments.Interfaces.Repositories;
using RabbitMQ.Domain.Payments.Queries.Results;
using RabbitMQ.Infra.Data.Repositories.Queries;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Infra.Data.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly Settings _settings;
        private readonly DynamicParameters _parameters = new DynamicParameters();

        public PaymentRepository(Settings settings)
        {
            _settings = settings;
        }

        public async Task SaveAsync(Payment payment)
        {
            _parameters.Add("Id", payment.Id, DbType.Guid);
            _parameters.Add("BarCode", payment.BarCode, DbType.String);
            _parameters.Add("Value", payment.Value, DbType.Decimal);
            _parameters.Add("Date", payment.Date, DbType.DateTime);
            _parameters.Add("Email", payment.Email, DbType.String);
            _parameters.Add("Reversed", payment.Reversed, DbType.Boolean);
            _parameters.Add("CreationDate", payment.CreationDate, DbType.DateTime);
            _parameters.Add("ChangeDate", payment.ChangeDate, DbType.DateTime);

            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                await connection.ExecuteAsync(PaymentQueries.Save, _parameters);
            }
        }

        public async Task UpdateAsync(Payment payment)
        {
            _parameters.Add("Id", payment.Id, DbType.Guid);
            _parameters.Add("BarCode", payment.BarCode, DbType.String);
            _parameters.Add("Value", payment.Value, DbType.Decimal);
            _parameters.Add("Date", payment.Date, DbType.DateTime);
            _parameters.Add("Email", payment.Email, DbType.String);
            _parameters.Add("Reversed", payment.Reversed, DbType.Boolean);
            _parameters.Add("CreationDate", payment.CreationDate, DbType.DateTime);
            _parameters.Add("ChangeDate", payment.ChangeDate, DbType.DateTime);

            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                await connection.ExecuteAsync(PaymentQueries.Update, _parameters);
            }
        }

        public async Task<PaymentQueryResult> GetAsync(Guid id)
        {
            _parameters.Add("Id", id, DbType.Guid);

            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                return (await connection.QueryAsync<PaymentQueryResult>(PaymentQueries.Get, _parameters)).FirstOrDefault();
            }
        }
    }
}