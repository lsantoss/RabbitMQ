using Dapper;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.QueueLogs;
using RabbitMQ.Domain.Core.QueueLogs.Interfaces.Repositories;
using RabbitMQ.Infra.Data.Repositories.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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

        public long Log(QueueLog queueLog)
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
                return connection.ExecuteScalar<long>(QueueLogQueries.Log, _parameters);
            }
        }

        public QueueLog Get(long id)
        {
            _parameters.Add("Id", id, DbType.Int64);

            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                return connection.Query<QueueLog>(QueueLogQueries.Get, _parameters).FirstOrDefault();
            }
        }

        public List<QueueLog> List(Guid idPagamento)
        {
            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                return connection.Query<QueueLog>(QueueLogQueries.List).ToList();
            }
        }
    }
}