namespace RabbitMQ.Domain.Core.Constants
{
    public static class ApplicationName
    {
        public static string PublisherPayments { get; } = "RabbitMQ.PublisherPayments";
        public static string PublisherReversals { get; } = "RabbitMQ.PublisherReversals";
        public static string PublisherEmails { get; } = "RabbitMQ.PublisherEmails";

        public static string ConsumerPayments { get; } = "RabbitMQ.ConsumerPayments";
        public static string ConsumerReversals { get; } = "RabbitMQ.ConsumerReversals";

        public static string EmailNotifier { get; } = "RabbitMQ.EmailNotifier";
    }
}