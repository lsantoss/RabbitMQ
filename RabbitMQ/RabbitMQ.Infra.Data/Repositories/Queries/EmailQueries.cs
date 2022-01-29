namespace RabbitMQ.Infra.Data.Repositories.Queries
{
    public static class EmailQueries
    {
        public static string Save { get; } = @"INSERT INTO Email (
                                                 Id,
                                                 PaymentId,
                                                 Notified,
                                                 CreationDate)
                                               VALUES(
                                                 @Id,
                                                 @PaymentId,
                                                 @Notified,
                                                 @CreationDate);";
    }
}