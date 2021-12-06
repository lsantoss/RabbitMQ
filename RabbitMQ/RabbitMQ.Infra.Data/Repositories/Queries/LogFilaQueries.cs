namespace RabbitMQ.Infra.Data.Repositories.Queries
{
    public static class LogFilaQueries
    {
        public static string Salvar { get; } = @"INSERT INTO LogFila (
                                                    IdPagamento, 
                                                    Worker,
                                                    Fila, 
                                                    Mensagem,
                                                    DataHora, 
                                                    Sucesso, 
                                                    NumeroTentativas,
                                                    Erro)
                                                VALUES(
                                                    @IdPagamento, 
                                                    @Worker,
                                                    @Fila, 
                                                    @Mensagem,
                                                    @DataHora, 
                                                    @Sucesso, 
                                                    @NumeroTentativas,
                                                    @Erro); 
                                                SELECT SCOPE_IDENTITY();";

        public static string Obter { get; } = @"SELECT 
                                                    LogFila.Id AS Id,
                                                    LogFila.IdPagamento AS IdPagamento, 
                                                    LogFila.Worker AS Worker,
                                                    LogFila.Fila AS Fila, 
                                                    LogFila.Mensagem AS Mensagem,
                                                    LogFila.DataHora AS DataHora, 
                                                    LogFila.Sucesso AS Sucesso, 
                                                    LogFila.NumeroTentativas AS NumeroTentativas,
                                                    LogFila.Erro AS Erro
                                                FROM LogFila WITH(NOLOCK)
                                                WHERE LogFila.Id = @Id";

        public static string Listar { get; } = @"SELECT 
                                                    LogFila.Id AS Id,
                                                    LogFila.IdPagamento AS IdPagamento, 
                                                    LogFila.Worker AS Worker,
                                                    LogFila.Fila AS Fila, 
                                                    LogFila.Mensagem AS Mensagem,
                                                    LogFila.DataHora AS DataHora, 
                                                    LogFila.Sucesso AS Sucesso, 
                                                    LogFila.NumeroTentativas AS NumeroTentativas,
                                                    LogFila.Erro AS Erro
                                                FROM LogFila WITH(NOLOCK) 
                                                WHERE LogFila.IdPagamento = @IdPagamento
                                                ORDER BY LogFila.Id ASC";
    }
}