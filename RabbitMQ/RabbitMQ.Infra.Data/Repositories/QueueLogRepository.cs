using Dapper;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.QueueLogs.Entities;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Domain.Core.QueueLogs.Queries.Results;
using RabbitMQ.Infra.Data.Repositories.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Infra.Data.Repositories
{
    public class QueueLogRepository : IQueueLogRepository
    {
        private readonly Settings _settings;
        private readonly DynamicParameters _parameters = new DynamicParameters();

        public QueueLogRepository(Settings settings)
        {
            _settings = settings;
        }

        public async Task<ulong> LogAsync(QueueLog queueLog)
        {
            _parameters.Add("PaymentId", queueLog.PaymentId, DbType.Guid);
            _parameters.Add("Worker", queueLog.Worker, DbType.String);
            _parameters.Add("Queue", queueLog.Queue, DbType.String);
            _parameters.Add("Message", queueLog.Message, DbType.String);
            _parameters.Add("Date", queueLog.Date, DbType.DateTime);
            _parameters.Add("Success", queueLog.Success, DbType.Boolean);
            _parameters.Add("NumberAttempts", queueLog.NumberAttempts, DbType.Int16);
            _parameters.Add("Error", queueLog.Error, DbType.String);

            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                return await connection.ExecuteScalarAsync<ulong>(QueueLogQueries.Log, _parameters);
            }
        }

        public async Task<List<QueueLogQueryResult>> ListAsync(Guid paymentId)
        {
            _parameters.Add("PaymentId", paymentId, DbType.Guid);

            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                return (await connection.QueryAsync<QueueLogQueryResult>(QueueLogQueries.List, _parameters)).ToList();
            }
        }
    }
}