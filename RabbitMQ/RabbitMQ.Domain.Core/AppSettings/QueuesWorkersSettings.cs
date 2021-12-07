namespace RabbitMQ.Domain.Core.AppSettings
{
    public class QueuesWorkersSettings
    {
        public string PublisherPayments { get; set; }
        public string PublisherReversals { get; set; }
        public string ConsumerPayments { get; set; }
        public string ConsumerReversals { get; set; }
        public string EmailNotifier { get; set; }
    }
}