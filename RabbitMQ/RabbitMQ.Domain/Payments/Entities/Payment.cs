using System;

namespace RabbitMQ.Domain.Payments.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public string BarCode { get; private set; }
        public decimal Value { get; private set; }
        public DateTime Date { get; private set; }
        public string Email { get; private set; }
        public bool Reversed { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime? ChangeDate { get; private set; }

        public Payment(Guid id, string barCode, decimal value, DateTime date, string email)
        {
            SetId(id);
            SetBarCode(barCode);
            SetValue(value);
            SetDate(date);
            SetEmail(email);
            SetReversed(false);
            SetCreationDate(DateTime.Now);
            SetChangeDate(null);
        }

        public Payment(Guid id, string barCode, decimal value, DateTime date, string email, bool reversed, DateTime creationDate, DateTime? changeDate)
        {
            SetId(id);
            SetBarCode(barCode);
            SetValue(value);
            SetDate(date);
            SetEmail(email);
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
        public void SetEmail(string email) => Email = email;
        public void SetReversed(bool reversed) => Reversed = reversed;
        public void SetCreationDate(DateTime creationDate) => CreationDate = creationDate;
        public void SetChangeDate(DateTime? changeDate) => ChangeDate = changeDate;
    }
}