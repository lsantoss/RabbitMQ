using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces.Repository;
using RabbitMQ.Domain.Core.Extensions;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMQ.Domain.Core.RabbitMQ.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly AutoResetEvent _waitHandle;
        private readonly ConnectionFactory _factory;

        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly IElmahRepository _elmahRepository;

        public RabbitMQService(RabbitMQSettings rabbitMQSettings, IElmahRepository elmahRepository)
        {
            _rabbitMQSettings = rabbitMQSettings;
            _elmahRepository = elmahRepository;

            _waitHandle = new(false);

            _factory = new ConnectionFactory()
            {
                HostName = _rabbitMQSettings.HostName,
                VirtualHost = _rabbitMQSettings.VirtualHost,
                Port = _rabbitMQSettings.Port,
                UserName = _rabbitMQSettings.UserName,
                Password = _rabbitMQSettings.Password,
                Uri = new Uri(_rabbitMQSettings.URL),
                AutomaticRecoveryEnabled = _rabbitMQSettings.AutomaticRecoveryEnabled
            };
        }

        public void Publish<T>(T message, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false)
        {
            try
            {
                using var connection = _factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.ConfirmSelect();

                _ = channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Headers = new Dictionary<string, object>();
                properties.Headers.Add("type-queue", Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName));

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                channel.BasicPublish(string.Empty, queueName, properties, body);

                channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to publish to RabbitMQ queue:", ex);
            }
        }

        public void Publish(string message, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false)
        {
            try
            {
                using var connection = _factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.ConfirmSelect();

                _ = channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

                message = message.RemoveJsonFormatting();

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Headers = new Dictionary<string, object>();
                properties.Headers.Add("type-queue", Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName));

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(string.Empty, queueName, properties, body);

                channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to publish to RabbitMQ queue:", ex);
            }
        }

        public void PublishDelayed<T>(T message, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false, int delayTime = 15000)
        {
            try
            {
                using var connection = _factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.ConfirmSelect();

                var queueNameDelayed = $"{queueName}_DELAYED";

                var arguments = new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", "" },
                    { "x-dead-letter-routing-key", queueName },
                    { "x-message-ttl", delayTime },
                    { "type-queue", Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName) }
                };

                _ = channel.QueueDeclare(queueNameDelayed, durable, exclusive, autoDelete, arguments);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Headers = new Dictionary<string, object>();
                properties.Headers.Add("type-queue", Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName));

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                channel.BasicPublish("", queueNameDelayed, properties, body);

                channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to publish to the RabbitMQ delayed queue:", ex);
            }
        }

        public void PublishDelayed(string message, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false, int delayTime = 15000)
        {
            try
            {
                using var connection = _factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.ConfirmSelect();

                var queueNameDelayed = $"{queueName}_DELAYED";

                var arguments = new Dictionary<string, object>
                {
                    { "x-dead-letter-exchange", "" },
                    { "x-dead-letter-routing-key", queueName },
                    { "x-message-ttl", delayTime },
                    { "type-queue", Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName) }
                };

                _ = channel.QueueDeclare(queueNameDelayed, durable, exclusive, autoDelete, arguments);

                message = message.RemoveJsonFormatting();

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Headers = new Dictionary<string, object>();
                properties.Headers.Add("type-queue", Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName));

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("", queueNameDelayed, properties, body);

                channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to publish to the RabbitMQ delayed queue:", ex);
            }
        }

        public void Consume(object handler, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            _ = channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                try
                {
                    var header = e.BasicProperties.Headers["type-queue"];

                    if (header != null)
                    {
                        var eventType = Type.GetType(Encoding.UTF8.GetString(header as byte[]));
                        var body = Encoding.UTF8.GetString(e.Body.ToArray());
                        var message = JsonConvert.DeserializeObject(body, eventType);

                        ((dynamic)handler).HandleAsync((dynamic)message).GetAwaiter().GetResult();

                        channel.BasicAck(e.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    channel.BasicNack(e.DeliveryTag, false, false);
                    throw new Exception("Failed to consume RabbitMQ queue:", ex);
                }
            };

            channel.BasicQos(_rabbitMQSettings.PrefetchSize, _rabbitMQSettings.PrefetchCount, false);
            _ = channel.BasicConsume(queueName, false, Environment.MachineName, consumer);

            Console.CancelKeyPress += (o, e) =>
            {
                if (_elmahRepository != null)
                    _ = _elmahRepository.LogAsync(new Exception("Closing RabbitMQ queue consumption (ctrl+C)"));

                _ = _waitHandle.Set();
            };

            _ = _waitHandle.WaitOne();
        }

        public void Consume<T>(object handler, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            _ = channel.QueueDeclare(queueName, true, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                try
                {
                    var header = e.BasicProperties.Headers["type-queue"];

                    if (header != null)
                    {
                        var body = Encoding.UTF8.GetString(e.Body.ToArray());
                        var message = JsonConvert.DeserializeObject(body, typeof(T));

                        ((dynamic)handler).HandleAsync((dynamic)message).GetAwaiter().GetResult();

                        channel.BasicAck(e.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    channel.BasicNack(e.DeliveryTag, false, false);
                    throw new Exception("Failed to consume RabbitMQ queue:", ex);
                }
            };

            channel.BasicQos(_rabbitMQSettings.PrefetchSize, _rabbitMQSettings.PrefetchCount, false);
            _ = channel.BasicConsume(queueName, false, Environment.MachineName, consumer);

            Console.CancelKeyPress += (o, e) =>
            {
                if (_elmahRepository != null)
                    _ = _elmahRepository.LogAsync(new Exception("Closing RabbitMQ queue consumption (ctrl+C)"));

                _ = _waitHandle.Set();
            };

            _ = _waitHandle.WaitOne();
        }
    }
}