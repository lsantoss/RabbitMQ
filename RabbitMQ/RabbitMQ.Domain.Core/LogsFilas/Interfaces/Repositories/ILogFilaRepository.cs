using System;
using System.Collections.Generic;

namespace RabbitMQ.Domain.Core.LogsFilas.Interfaces.Repositories
{
    public interface ILogFilaRepository
    {
        long Inserir(LogFila logFila);
        LogFila Obter(long id);
        List<LogFila> Listar(Guid idPagamento);
    }
}