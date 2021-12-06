using Dapper;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.LogsFilas;
using RabbitMQ.Domain.Core.LogsFilas.Interfaces.Repositories;
using RabbitMQ.Infra.Data.Repositories.Queries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace RabbitMQ.Infra.Data.Repositories
{
    public class LogFilaRepository : ILogFilaRepository
    {
        private readonly Settings _settings;
        private readonly DynamicParameters _parametros = new DynamicParameters();

        public LogFilaRepository(Settings settings)
        {
            _settings = settings;
        }

        public long Inserir(LogFila logFila)
        {
            _parametros.Add("IdPagamento", logFila.IdPagamento, DbType.Guid);
            _parametros.Add("Worker", logFila.Worker, DbType.String);
            _parametros.Add("Fila", logFila.Fila, DbType.String);
            _parametros.Add("Mensagem", logFila.Mensagem, DbType.String);
            _parametros.Add("DataHora", logFila.DataHora, DbType.DateTime);
            _parametros.Add("Sucesso", logFila.Sucesso, DbType.Boolean);
            _parametros.Add("NumeroTentativas", logFila.NumeroTentativas, DbType.Int16);
            _parametros.Add("Erro", logFila.Erro, DbType.String);

            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                return connection.ExecuteScalar<long>(LogFilaQueries.Salvar, _parametros);
            }
        }

        public LogFila Obter(long id)
        {
            _parametros.Add("Id", id, DbType.Int64);

            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                return connection.Query<LogFila>(LogFilaQueries.Obter, _parametros).FirstOrDefault();
            }
        }

        public List<LogFila> Listar(Guid idPagamento)
        {
            using (var connection = new SqlConnection(_settings.ConnectionString))
            {
                return connection.Query<LogFila>(LogFilaQueries.Listar).ToList();
            }
        }
    }
}