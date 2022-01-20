using System;

namespace RabbitMQ.Domain.Payments.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public string BarCode { get; private set; }
        public decimal Value { get; private set; }
        public DateTime Date { get; private set; }
        public string ClientName { get; private set; }
        public string ClientEmail { get; private set; }
        public string ClientCellphone { get; private set; }
        public bool NotifyByEmail { get; private set; }
        public bool NotifyBySMS { get; private set; }
        public bool Reversed { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime? ChangeDate { get; private set; }

        public Payment(Guid id,
                       string barCode,
                       decimal value,
                       DateTime date,
                       string clientName,
                       string clientEmail,
                       string clientCellphone,
                       bool notifyByEmail,
                       bool notifyBySMS)
        {
            SetId(id);
            SetBarCode(barCode);
            SetValue(value);
            SetDate(date);
            SetClientName(clientName);
            SetClientEmail(clientEmail);
            SetClientCellphone(clientCellphone);
            SetNotifyByEmail(notifyByEmail);
            SetNotifyBySMS(notifyBySMS);
            SetReversed(false);
            SetCreationDate(DateTime.Now);
            SetChangeDate(null);
        }

        public Payment(Guid id,
                       string barCode,
                       decimal value,
                       DateTime date,
                       string clientName,
                       string clientEmail,
                       string clientCellphone,
                       bool notifyByEmail,
                       bool notifyBySMS,
                       bool reversed,
                       DateTime creationDate,
                       DateTime? changeDate)
        {
            SetId(id);
            SetBarCode(barCode);
            SetValue(value);
            SetDate(date);
            SetClientName(clientName);
            SetClientEmail(clientEmail);
            SetClientCellphone(clientCellphone);
            SetNotifyByEmail(notifyByEmail);
            SetNotifyBySMS(notifyBySMS);
            SetReversed(reversed);
            SetCreationDate(creationDate);
            SetChangeDate(changeDate);
        }

        public void Reverse()
        {
            SetReversed(true);
            SetChangeDate(DateTime.Now);
        }

        public void SetId(Guid id) => Id = id;
        public void SetBarCode(string barCode) => BarCode = barCode;
        public void SetValue(decimal value) => Value = value;
        public void SetDate(DateTime date) => Date = date;
        public void SetClientName(string clientName) => ClientName = clientName;
        public void SetClientEmail(string clientEmail) => ClientEmail = clientEmail;
        public void SetClientCellphone(string clientCellphone) => ClientCellphone = clientCellphone;
        public void SetNotifyByEmail(bool notifyByEmail) => NotifyByEmail = notifyByEmail;
        public void SetNotifyBySMS(bool notifyBySMS) => NotifyBySMS = notifyBySMS;
        public void SetReversed(bool reversed) => Reversed = reversed;
        public void SetCreationDate(DateTime creationDate) => CreationDate = creationDate;
        public void SetChangeDate(DateTime? changeDate) => ChangeDate = changeDate;
    }
}