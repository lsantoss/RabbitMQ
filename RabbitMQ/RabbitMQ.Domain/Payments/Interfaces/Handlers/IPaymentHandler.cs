﻿using RabbitMQ.Domain.Payments.Commands.Inputs;
using System.Threading.Tasks;

namespace RabbitMQ.Domain.Payments.Interfaces.Handlers
{
    public interface IPaymentHandler
    {
        Task Handle(PaymentCommand paymentCommand);
    }
}