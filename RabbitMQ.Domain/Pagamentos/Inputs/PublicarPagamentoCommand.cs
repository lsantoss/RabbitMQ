using System;

namespace RabbitMQ.Domain.Pagamentos.Inputs
{
    public class PublicarPagamentoCommand
    {
        public Guid Id { get; set; }
        public string CodigoBarras { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataHora { get; set; }
    }
}