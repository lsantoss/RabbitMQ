using ElmahCore;
using ElmahCore.Sql;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using System;

namespace RabbitMQ.Infra.Data.Repositories
{
    public class ElmahRepository : IElmahRepository
    {
        private readonly ErrorLog _errorLog;

        public ElmahRepository(Settings settings)
        {
            _errorLog = new SqlErrorLog(settings.ConnectionString)
            {
                ApplicationName = settings.ApplicationName
            };
        }

        public string LogarErro(Error erro)
        {
            try
            {
                return _errorLog.Log(erro);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message, ex);
            }
        }
    }
}