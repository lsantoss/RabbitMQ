namespace RabbitMQ.Domain.Core.Constants
{
    public static class QueueName
    {
        public static string Payment { get; } = "RabbitMQ.Payment";
        public static string Reversal { get; } = "RabbitMQ.Reversal";
        public static string Email { get; } = "RabbitMQ.Email";
    }
}