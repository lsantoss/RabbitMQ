using ElmahCore;
using Newtonsoft.Json;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Domain.Pagamentos.Inputs;
using RabbitMQ.Infra.Crosscutting;
using System;
using System.IO;

namespace RabbitMQ.Publisher
{
    class Program
    {
        private static readonly WorkerBase _workerBase;
        private static readonly Settings _settings;
        private static readonly FilasWorkersSettings _filasWorkers;
        private static readonly IElmahRepository _elmahRepository;
        private static readonly IRabbitMQBus _rabbitMQBus;

        static Program()
        {
            _workerBase = new WorkerBase(EFila.PublicarPagamento);
            _settings = _workerBase.GetService<Settings>();
            _filasWorkers = _workerBase.GetService<FilasWorkersSettings>();
            _elmahRepository = _workerBase.GetService<IElmahRepository>();
            _rabbitMQBus = _workerBase.GetService<IRabbitMQBus>();
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Iniciando Worker {_settings.ApplicationName} \n");

                var jsonParaEnvioPath = $@"{AppDomain.CurrentDomain.BaseDirectory}\payload.json";
                var json = "";

                using (var streamReader = new StreamReader(jsonParaEnvioPath))
                {
                    json = streamReader.ReadToEnd();
                }

                Console.WriteLine(json);

                var payload = JsonConvert.DeserializeObject<PublicarPagamentoCommand>(json);

                _rabbitMQBus.Publicar(payload, _filasWorkers.PublicarPagamento);

                Console.WriteLine("\n\nMensagem enviada com sucesso!");
                Console.ReadKey();
            }
            catch(Exception ex)
            {
                _elmahRepository.LogarErro(new Error(ex));
                Console.WriteLine($"\n\nErro ao enviar mensagem! {ex.InnerException.Message}");
                Console.ReadKey();
            }
        }
    }
}