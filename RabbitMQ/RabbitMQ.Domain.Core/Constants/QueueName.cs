namespace RabbitMQ.Domain.Core.Constants
{
    public static class QueueName
    {
        public static string Payments { get; } = "RabbitMQ.Payments";
        public static string Reversals { get; } = "RabbitMQ.Reversals";
        public static string EmailNotifier { get; } = "RabbitMQ.Email";
    }
}