namespace RabbitMQ.Infra.Data.Repositories.Queries
{
    public static class PaymentQueries
    {
        public static string Save { get; } = @"INSERT INTO Payment (
                                                Id, 
                                                BarCode,
                                                Value, 
                                                Date, 
                                                Email, 
                                                Reversed,
                                                CreationDate,
                                                ChangeDate)
                                              VALUES(
                                                @Id, 
                                                @BarCode,
                                                @Value, 
                                                @Date, 
                                                @Email, 
                                                @Reversed,
                                                @CreationDate,
                                                @ChangeDate);";

        public static string Update { get; } = @"UPDATE Payment SET
                                                    BarCode = @BarCode,
                                                    Value = @Value,
                                                    Date = @Date,
                                                    Email = @Email,
                                                    Reversed = @Reversed,
                                                    CreationDate = @CreationDate,
                                                    ChangeDate = @ChangeDate 
                                                WHERE Id = @Id";

        public static string Get { get; } = @"SELECT 
                                                Payment.Id AS Id, 
                                                Payment.BarCode AS BarCode,
                                                Payment.Value AS Value, 
                                                Payment.Date AS Date, 
                                                Payment.Email AS Email, 
                                                Payment.CreationDate AS CreationDate,
                                                Payment.Reversed AS Reversed,
                                                Payment.ChangeDate AS ChangeDate
                                              FROM Payment WITH(NOLOCK)
                                              WHERE Payment.Id = @Id";
    }
}