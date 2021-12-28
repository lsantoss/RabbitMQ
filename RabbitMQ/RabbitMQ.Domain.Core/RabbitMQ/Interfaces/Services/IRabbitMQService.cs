namespace RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services
{
    public interface IRabbitMQService
    {
        void Publish<T>(T message, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false);
        void Publish(string message, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false);
        void PublishDelayed<T>(T message, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false, int delayTime = 15000);
        void PublishDelayed(string message, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false, int delayTime = 15000);
        void Consume(object handler, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false);
        void Consume<T>(object handler, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false);
    }
}