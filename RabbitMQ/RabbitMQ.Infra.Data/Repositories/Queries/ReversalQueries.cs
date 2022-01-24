namespace RabbitMQ.Infra.Data.Repositories.Queries
{
    public static class ReversalQueries
    {
        public static string Save { get; } = @"INSERT INTO Reversal (
                                                 Id,
                                                 PaymentId,
                                                 Date,
                                                 CreationDate)
                                               VALUES(
                                                 @Id,
                                                 @PaymentId,
                                                 @Date,
                                                 @CreationDate);";

        public static string Get { get; } = @"SELECT 
                                                Reversal.Id AS Id, 
                                                Reversal.PaymentId AS PaymentId,
                                                Reversal.Date AS Date, 
                                                Reversal.CreationDate AS CreationDate
                                              FROM Reversal WITH(NOLOCK)
                                              WHERE Reversal.PaymentId = @PaymentId";
    }
}