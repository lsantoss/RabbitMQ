using ElmahCore;
using ElmahCore.Sql;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using System;

namespace RabbitMQ.Infra.Data.Repositories
{
    public class ElmahRepository : IElmahRepository
    {
        private readonly ErrorLog _elmahLog;

        public ElmahRepository(Settings settings)
        {
            _elmahLog = new SqlErrorLog(settings.ConnectionString)
            {
                ApplicationName = settings.ApplicationName
            };
        }

        public string LogarErro(Error erro)
        {
            try
            {
                return _elmahLog.Log(erro);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message, ex);
            }
        }
    }
}