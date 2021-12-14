using ElmahCore;
using ElmahCore.Sql;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using System;
using System.Threading.Tasks;

namespace RabbitMQ.Infra.Data.Repositories
{
    public class ElmahRepository : IElmahRepository
    {
        private readonly ErrorLog _elmahLog;

        public ElmahRepository(Settings settings)
        {
            _elmahLog = new SqlErrorLog(settings.ConnectionString);
        }

        public async Task<string> Log(Error erro)
        {
            try
            {
                return await _elmahLog.LogAsync(erro);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message, ex);
            }
        }
    }
}