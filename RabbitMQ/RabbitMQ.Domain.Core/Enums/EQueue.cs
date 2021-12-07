namespace RabbitMQ.Domain.Core.Enums
{
    public enum EQueue
    {
        PublisherPayments = 1,
        PublisherReversals = 2,
        ConsumerPayments = 3,
        ConsumerReversals = 4,
        EmailNotifier = 5
    }
}