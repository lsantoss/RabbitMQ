namespace RabbitMQ.Infra.Data.Repositories.Queries
{
    public static class PaymentQueries
    {
        public static string Save { get; } = @"INSERT INTO Payment (
                                                Id, 
                                                BarCode,
                                                Value, 
                                                Date, 
                                                ClientName, 
                                                ClientEmail, 
                                                ClientCellphone, 
                                                NotifyByEmail, 
                                                NotifyBySMS, 
                                                Reversed,
                                                CreationDate,
                                                ChangeDate)
                                              VALUES(
                                                @Id, 
                                                @BarCode,
                                                @Value, 
                                                @Date, 
                                                @ClientName, 
                                                @ClientEmail, 
                                                @ClientCellphone, 
                                                @NotifyByEmail, 
                                                @NotifyBySMS, 
                                                @Reversed,
                                                @CreationDate,
                                                @ChangeDate);";

        public static string Update { get; } = @"UPDATE Payment SET
                                                    BarCode = @BarCode,
                                                    Value = @Value,
                                                    Date = @Date,
                                                    ClientName = @ClientName,
                                                    ClientEmail = @ClientEmail,
                                                    ClientCellphone = @ClientCellphone,
                                                    NotifyByEmail = @NotifyByEmail,
                                                    NotifyBySMS = @NotifyBySMS,
                                                    Reversed = @Reversed,
                                                    CreationDate = @CreationDate,
                                                    ChangeDate = @ChangeDate 
                                                 WHERE Id = @Id";

        public static string Get { get; } = @"SELECT 
                                                Payment.Id AS Id, 
                                                Payment.BarCode AS BarCode,
                                                Payment.Value AS Value, 
                                                Payment.Date AS Date, 
                                                Payment.ClientName AS ClientName, 
                                                Payment.ClientEmail AS ClientEmail, 
                                                Payment.ClientCellphone AS ClientCellphone, 
                                                Payment.NotifyByEmail AS NotifyByEmail, 
                                                Payment.NotifyBySMS AS NotifyBySMS, 
                                                Payment.Reversed AS Reversed,
                                                Payment.CreationDate AS CreationDate,
                                                Payment.ChangeDate AS ChangeDate
                                              FROM Payment WITH(NOLOCK)
                                              WHERE Payment.Id = @Id";
    }
}