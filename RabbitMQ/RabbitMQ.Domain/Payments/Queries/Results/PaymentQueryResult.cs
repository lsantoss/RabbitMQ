using RabbitMQ.Domain.Payments.Entities;
using System;

namespace RabbitMQ.Domain.Payments.Queries.Results
{
    public class PaymentQueryResult
    {
        public Guid Id { get; set; }
        public string BarCode { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientCellphone { get; set; }
        public bool NotifyByEmail { get; set; }
        public bool NotifyByCellphone { get; set; }
        public bool Reversed { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ChangeDate { get; set; }

        public Payment MapToPayment() => new(Id, BarCode, Value, Date, ClientName, ClientEmail, ClientCellphone, 
                                             NotifyByEmail, NotifyByCellphone, Reversed, CreationDate, ChangeDate);
    }
}