﻿using System;

namespace RabbitMQ.Domain.Common.Commands.Inputs
{
    public abstract class Command
    {
        public abstract Guid Id { get; set; }
        public abstract byte NumberAttempts { get; set; }

        public abstract void AddNumberAttempt();
    }
}