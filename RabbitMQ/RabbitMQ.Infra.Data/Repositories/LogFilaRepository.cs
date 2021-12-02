using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.LogsFilas;
using RabbitMQ.Domain.Core.LogsFilas.Interfaces.Repositories;
using System;
using System.Collections.Generic;

namespace RabbitMQ.Infra.Data.Repositories
{
    public class LogFilaRepository : ILogFilaRepository
    {
        private readonly Settings _settings;

        public LogFilaRepository(Settings settings)
        {
            _settings = settings;
        }

        public long Inserir(LogFila logFila)
        {
            throw new NotImplementedException();
        }

        public void Atualizar(LogFila logFila)
        {
            throw new NotImplementedException();
        }

        public LogFila Obter(long id)
        {
            throw new NotImplementedException();
        }

        public List<LogFila> Listar(Guid idPagamento)
        {
            throw new NotImplementedException();
        }
    }
}