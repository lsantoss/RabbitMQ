﻿using Dapper;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Reversals.Entities;
using RabbitMQ.Domain.Reversals.Interfaces.Repositories;
using RabbitMQ.Domain.Reversals.Queries.Results;
using RabbitMQ.Infra.Data.Repositories.Queries;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RabbitMQ.Infra.Data.Repositories
{
    public class ReversalRepository : IReversalRepository
    {
        private readonly DynamicParameters _parameters = new();

        private readonly Settings _settings;

        public ReversalRepository(Settings settings)
        {
            _settings = settings;
        }

        public async Task SaveAsync(Reversal reversal)
        {
            _parameters.Add("Id", reversal.Id, DbType.Guid);
            _parameters.Add("PaymentId", reversal.PaymentId, DbType.Guid);
            _parameters.Add("Date", reversal.Date, DbType.DateTime);
            _parameters.Add("CreationDate", reversal.CreationDate, DbType.DateTime);

            using var connection = new SqlConnection(_settings.ConnectionString);

            await connection.ExecuteAsync(ReversalQueries.Save, _parameters);
        }

        public async Task<ReversalQueryResult> GetAsync(Guid paymentId)
        {
            _parameters.Add("PaymentId", paymentId, DbType.Guid);

            using var connection = new SqlConnection(_settings.ConnectionString);

            return await connection.QueryFirstOrDefaultAsync<ReversalQueryResult>(ReversalQueries.Get, _parameters);
        }
    }
}