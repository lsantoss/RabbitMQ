using ElmahCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
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

        public void Publicar<T>(T @event, string nomeFila, bool duravel = true, bool excluivel = false, bool apagaAutomaticamente = false)
        {
            try
            {
                using (var conexao = Factory.CreateConnection($"{Environment.MachineName}_{nomeFila}"))
                using (var canal = conexao.CreateModel())
                {
                    canal.ConfirmSelect();

                    canal.QueueDeclare(nomeFila, duravel, excluivel, apagaAutomaticamente, null);

                    var mensagem = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));

                    var propriedades = canal.CreateBasicProperties();
                    propriedades.Persistent = true;
                    propriedades.Headers = new Dictionary<string, object>();
                    propriedades.Headers.Add("type-queue", Encoding.UTF8.GetBytes(@event.GetType().AssemblyQualifiedName));

                    canal.BasicPublish(string.Empty, nomeFila, propriedades, mensagem);

                    canal.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao publicar na fila do RabbitMQ:", ex);
            }
        }

        public void PublicarComAtraso<T>(T @event, string nomeFila, bool duravel = true, bool excluivel = false, bool apagaAutomaticamente = false, int tempoAtraso = 15000)
        {
            try
            {
                using (var conexao = Factory.CreateConnection($"{Environment.MachineName}_{nomeFila}"))
                using (var canal = conexao.CreateModel())
                {
                    canal.ConfirmSelect();

                    var nomeFilaComAtraso = $"{nomeFila}_DELAYED";
                    var arguments = new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", "" },
                        { "x-dead-letter-routing-key", nomeFila },
                        { "x-message-ttl", tempoAtraso },
                        { "type-queue", Encoding.UTF8.GetBytes(@event.GetType().AssemblyQualifiedName) }
                    };

                    canal.QueueDeclare(nomeFilaComAtraso, duravel, excluivel, apagaAutomaticamente, arguments);

                    var mensagem = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));

                    var propriedades = canal.CreateBasicProperties();
                    propriedades.Persistent = true;
                    propriedades.Headers = new Dictionary<string, object>();
                    propriedades.Headers.Add("type-queue", Encoding.UTF8.GetBytes(@event.GetType().AssemblyQualifiedName));

                    canal.BasicPublish("", nomeFilaComAtraso, propriedades, mensagem);

                    canal.WaitForConfirmsOrDie(new TimeSpan(0, 0, 5));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao publicar na fila com delay do RabbitMQ:", ex);
            }
        }

        public void Consumir(object handler, string nomeFila, bool duravel = true, bool excluivel = false, bool apagaAutomaticamente = false)
        {
            using (var conexao = Factory.CreateConnection($"{Environment.MachineName}_{nomeFila}"))
            using (var canal = conexao.CreateModel())
            {
                canal.QueueDeclare(nomeFila, duravel, excluivel, apagaAutomaticamente, null);

                var consumidor = new EventingBasicConsumer(canal);
                consumidor.Received += (sender, e) =>
                {
                    try
                    {
                        var mensagem = Encoding.UTF8.GetString(e.Body.ToArray());

                        var propriedadesBasicas = e.BasicProperties;

                        var cabecalho = propriedadesBasicas.Headers["type-queue"];

                        if (cabecalho != null)
                        {
                            var tipoEvento = Type.GetType(Encoding.UTF8.GetString(cabecalho as byte[]));

                            var @event = JsonConvert.DeserializeObject(mensagem, tipoEvento);

                            ((dynamic)handler).Handle((dynamic)@event).GetAwaiter().GetResult();

                            canal.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                        }
                    }
                    catch (Exception ex)
                    {
                        canal.BasicNack(deliveryTag: e.DeliveryTag, multiple: false, requeue: false);
                        throw new Exception("Falha ao consumir a fila do RabbitMQ:", ex);
                    }
                };

                canal.BasicQos(_rabbitMQSettings.PrefetchSize, _rabbitMQSettings.PrefetchCount, false);
                canal.BasicConsume(nomeFila, false, Environment.MachineName, consumidor);

                Console.CancelKeyPress += (o, e) =>
                {
                    if (_elmahRepository != null)
                        _elmahRepository.LogarErro(new Error(new Exception("Encerrando consumo de fila do RabbitMQ (ctrl+C)")));

                    waitHandle.Set();
                };

                waitHandle.WaitOne();
            }
        }

        public void Consumir<T>(object handler, string nomeFila, bool duravel = true, bool excluivel = false, bool apagaAutomaticamente = false)
        {
            using (var conexao = Factory.CreateConnection($"{Environment.MachineName}_{nomeFila}"))
            using (var canal = conexao.CreateModel())
            {
                canal.QueueDeclare(nomeFila, true, false, false, null);

                var consumidor = new EventingBasicConsumer(canal);
                consumidor.Received += (sender, e) =>
                {
                    try
                    {
                        var mensagem = Encoding.UTF8.GetString(e.Body.ToArray());

                        var propriedadesBasicas = e.BasicProperties;

                        var cabecalho = propriedadesBasicas.Headers["type-queue"];

                        if (cabecalho != null)
                        {
                            var @event = JsonConvert.DeserializeObject(mensagem, typeof(T));

                            ((dynamic)handler).Handle((dynamic)@event).GetAwaiter().GetResult();

                            canal.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                        }
                    }
                    catch (Exception ex)
                    {
                        canal.BasicNack(deliveryTag: e.DeliveryTag, multiple: false, requeue: false);
                        throw new Exception("Falha ao consumir a fila do RabbitMQ:", ex);
                    }
                };

                canal.BasicQos(_rabbitMQSettings.PrefetchSize, _rabbitMQSettings.PrefetchCount, false);
                canal.BasicConsume(nomeFila, false, Environment.MachineName, consumidor);

                Console.CancelKeyPress += (o, e) =>
                {
                    if (_elmahRepository != null)
                        _elmahRepository.LogarErro(new Error(new Exception("Encerrando consumo de fila do RabbitMQ (ctrl+C)")));

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
                        AutomaticRecoveryEnabled = false
                    };
                }

                return _factory;
            }
        }
    }
}