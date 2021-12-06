using System;

namespace RabbitMQ.Domain.Core.LogsFilas
{
    public class LogFila
    {
        public ulong Id { get; private set; }
        public Guid IdPagamento { get; private set; }
        public string Worker { get; private set; }
        public string Fila { get; private set; }
        public string Mensagem { get; private set; }
        public DateTime DataHora { get; private set; }
        public bool Sucesso { get; private set; }
        public byte NumeroTentativas { get; private set; }
        public string Erro { get; private set; }

        public LogFila(Guid idPagamento, string worker, string fila, string mensagem)
        {
            Id = 0;
            IdPagamento = idPagamento;
            Worker = worker;
            Fila = fila;
            Mensagem = mensagem;
            DataHora = DateTime.Now;
            Sucesso = true;
            NumeroTentativas = 1;
            Erro = null;
        }

        public LogFila(Guid idPagamento, string worker, string fila, string mensagem, byte numeroTentativas, string erro)
        {
            Id = 0;
            IdPagamento = idPagamento;
            Worker = worker;
            Fila = fila;
            Mensagem = mensagem;
            DataHora = DateTime.Now;
            Sucesso = false;
            NumeroTentativas = numeroTentativas;
            Erro = erro;
        }
    }
}