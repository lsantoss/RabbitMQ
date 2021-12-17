using ElmahCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.Extensions;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMQ.Domain.Core.RabbitMQ
{
    public class RabbitMQBus : IRabbitMQBus
    {
        private static AutoResetEvent waitHandle = new AutoResetEvent(false);
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly IElmahRepository _elmahRepository;

        public RabbitMQBus(RabbitMQSettings rabbitMQSettings, IElmahRepository elmahRepository)
        {
            _rabbitMQSettings = rabbitMQSettings;
            _elmahRepository = elmahRepository;
        }

        public void Publish<T>(T message, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false)
        {
            try
            {
                using (var connection = Factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ConfirmSelect();

                    channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.Headers = new Dictionary<string, object>();
                    properties.Headers.Add("type-queue", Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName));

                    channel.BasicPublish(string.Empty, queueName, properties, body);

                    channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
                }
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
                using (var connection = Factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ConfirmSelect();

                    channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

                    message = message.RemoveJsonFormatting();

                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.Headers = new Dictionary<string, object>();
                    properties.Headers.Add("type-queue", Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName));

                    channel.BasicPublish(string.Empty, queueName, properties, body);

                    channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
                }
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
                using (var connection = Factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ConfirmSelect();

                    var queueNameDelayed = $"{queueName}_DELAYED";
                    var arguments = new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", "" },
                        { "x-dead-letter-routing-key", queueName },
                        { "x-message-ttl", delayTime },
                        { "type-queue", Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName) }
                    };

                    channel.QueueDeclare(queueNameDelayed, durable, exclusive, autoDelete, arguments);

                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.Headers = new Dictionary<string, object>();
                    properties.Headers.Add("type-queue", Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName));

                    channel.BasicPublish("", queueNameDelayed, properties, body);

                    channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
                }
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
                using (var connection = Factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ConfirmSelect();

                    var queueNameDelayed = $"{queueName}_DELAYED";
                    var arguments = new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", "" },
                        { "x-dead-letter-routing-key", queueName },
                        { "x-message-ttl", delayTime },
                        { "type-queue", Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName) }
                    };

                    channel.QueueDeclare(queueNameDelayed, durable, exclusive, autoDelete, arguments);

                    message = message.RemoveJsonFormatting();

                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    properties.Headers = new Dictionary<string, object>();
                    properties.Headers.Add("type-queue", Encoding.UTF8.GetBytes(message.GetType().AssemblyQualifiedName));

                    channel.BasicPublish("", queueNameDelayed, properties, body);

                    channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to publish to the RabbitMQ delayed queue:", ex);
            }
        }

        public void Consume(object handler, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false)
        {
            using (var connection = Factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) =>
                {
                    try
                    {
                        var body = Encoding.UTF8.GetString(e.Body.ToArray());

                        var basicProperties = e.BasicProperties;

                        var header = basicProperties.Headers["type-queue"];

                        if (header != null)
                        {
                            var eventType = Type.GetType(Encoding.UTF8.GetString(header as byte[]));

                            var message = JsonConvert.DeserializeObject(body, eventType);

                            ((dynamic)handler).Handle((dynamic)message).GetAwaiter().GetResult();

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
                channel.BasicConsume(queueName, false, Environment.MachineName, consumer);

                Console.CancelKeyPress += (o, e) =>
                {
                    if (_elmahRepository != null)
                        _elmahRepository.Log(new Error(new Exception("Closing RabbitMQ queue consumption (ctrl+C)")));

                    waitHandle.Set();
                };

                waitHandle.WaitOne();
            }
        }

        public void Consume<T>(object handler, string queueName, bool durable = true, bool exclusive = false, bool autoDelete = false)
        {
            using (var connection = Factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queueName, true, false, false, null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) =>
                {
                    try
                    {
                        var body = Encoding.UTF8.GetString(e.Body.ToArray());

                        var basicProperties = e.BasicProperties;

                        var header = basicProperties.Headers["type-queue"];

                        if (header != null)
                        {
                            var message = JsonConvert.DeserializeObject(body, typeof(T));

                            ((dynamic)handler).Handle((dynamic)message).GetAwaiter().GetResult();

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
                channel.BasicConsume(queueName, false, Environment.MachineName, consumer);

                Console.CancelKeyPress += (o, e) =>
                {
                    if (_elmahRepository != null)
                        _elmahRepository.Log(new Error(new Exception("Closing RabbitMQ queue consumption (ctrl+C)")));

                    waitHandle.Set();
                };

                waitHandle.WaitOne();
            }
        }

        private ConnectionFactory _factory;
        private ConnectionFactory Factory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new ConnectionFactory()
                    {
                        HostName = _rabbitMQSettings.HostName,
                        VirtualHost = _rabbitMQSettings.VirtualHost,
                        Port = _rabbitMQSettings.Port,
                        UserName = _rabbitMQSettings.UserName,
                        Password = _rabbitMQSettings.Password,
                        Uri = new Uri(_rabbitMQSettings.URL),
                        AutomaticRecoveryEnabled = false
                    };
                }

                return _factory;
            }
        }
    }
}