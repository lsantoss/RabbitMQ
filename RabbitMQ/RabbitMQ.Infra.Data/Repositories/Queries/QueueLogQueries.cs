namespace RabbitMQ.Infra.Data.Repositories.Queries
{
    public static class QueueLogQueries
    {
        public static string Log { get; } = @"INSERT INTO QueueLog (
                                                PaymentId, 
                                                Worker,
                                                Queue, 
                                                Message,
                                                Date, 
                                                Success, 
                                                NumberAttempts,
                                                Error)
                                              VALUES(
                                                @PaymentId, 
                                                @Worker,
                                                @Queue, 
                                                @Message,
                                                @Date, 
                                                @Success, 
                                                @NumberAttempts,
                                                @Error); 
                                              SELECT SCOPE_IDENTITY();";

        public static string List { get; } = @"SELECT 
                                                 QueueLog.Id AS Id,
                                                 QueueLog.PaymentId AS PaymentId, 
                                                 QueueLog.Worker AS Worker,
                                                 QueueLog.Queue AS Queue, 
                                                 QueueLog.Message AS Message,
                                                 QueueLog.Date AS Date, 
                                                 QueueLog.Success AS Success, 
                                                 QueueLog.NumberAttempts AS NumberAttempts,
                                                 QueueLog.Error AS Error
                                               FROM QueueLog WITH(NOLOCK) 
                                               WHERE QueueLog.PaymentId = @PaymentId
                                               ORDER BY QueueLog.Id ASC";
    }
}