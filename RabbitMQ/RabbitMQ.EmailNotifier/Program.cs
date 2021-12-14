using RabbitMQ.Domain.Core.Constants;
using System;

namespace RabbitMQ.EmailNotifier
{
    class Program
    {
        private static readonly string _applicationName;
        private static readonly string _queueName;

        static Program()
        {
            _applicationName = ApplicationName.EmailNotifier;
            _queueName = QueueName.EmailNotifier;
        }

        static void Main(string[] args)
        {
            try
            {
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.ReadKey();
            }
        }
    }
}