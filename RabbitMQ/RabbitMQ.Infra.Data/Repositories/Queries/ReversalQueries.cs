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
    }
}