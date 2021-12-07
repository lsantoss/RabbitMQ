﻿using System;

namespace RabbitMQ.Domain.Payments.Commands.Inputs
{
    public class PublishPaymentCommand
    {
        public Guid Id { get; set; }
        public string BarCode { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
    }
}