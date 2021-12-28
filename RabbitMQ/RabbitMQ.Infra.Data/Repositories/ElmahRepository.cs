using ElmahCore;
using ElmahCore.Sql;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
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

        public async Task<string> LogAsync(Exception exception)
        {            
            try
            {
                return await _elmahLog.LogAsync(new Error(exception));
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message, ex);
            }
        }
    }
}