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
    }
}