using ElmahCore;
using Newtonsoft.Json;
using RabbitMQ.Domain.Core.AppSettings;
using RabbitMQ.Domain.Core.Elmah.Interfaces;
using RabbitMQ.Domain.Core.Enums;
using RabbitMQ.Domain.Core.Helpers;
using RabbitMQ.Domain.Core.LogsFilas;
using RabbitMQ.Domain.Core.LogsFilas.Interfaces.Repositories;
using RabbitMQ.Domain.Core.RabbitMQ.Interfaces;
using RabbitMQ.Domain.Pagamentos.Inputs;
using RabbitMQ.Infra.Crosscutting;
using System;

namespace RabbitMQ.Publisher
{
    class Program
    {
        private static readonly WorkerBase _workerBase;
        private static readonly Settings _settings;
        private static readonly FilasWorkersSettings _filasWorkers;
        private static readonly ILogFilaRepository _logFilaRepository;
        private static readonly IElmahRepository _elmahRepository;
        private static readonly IRabbitMQBus _rabbitMQBus;

        static Program()
        {
            _workerBase = new WorkerBase(EFila.PublicarPagamento);
            _settings = _workerBase.GetService<Settings>();
            _filasWorkers = _workerBase.GetService<FilasWorkersSettings>();
            _logFilaRepository = _workerBase.GetService<ILogFilaRepository>();
            _elmahRepository = _workerBase.GetService<IElmahRepository>();
            _rabbitMQBus = _workerBase.GetService<IRabbitMQBus>();
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine($"Iniciando Worker {_settings.ApplicationName} \n");

                var arquivoPath = $@"{AppDomain.CurrentDomain.BaseDirectory}\payload.json";
                
                var pagamentoJson = LeitorArquivosHelper.Ler(arquivoPath);

                Console.WriteLine(pagamentoJson);

                var pagamento = JsonConvert.DeserializeObject<PublicarPagamentoCommand>(pagamentoJson);

                _rabbitMQBus.Publicar(pagamento, _filasWorkers.PublicarPagamento);

                var logFila = new LogFila(pagamento.Id, _settings.ApplicationName, _filasWorkers.PublicarPagamento, pagamentoJson);
                _logFilaRepository.Inserir(logFila);

                Console.WriteLine("\n\nMensagem enviada com sucesso!");
                Console.ReadKey();
            }
            catch(Exception ex)
            {
                _elmahRepository.LogarErro(new Error(ex));
                Console.WriteLine($"\n\nErro ao enviar mensagem! {ex.Message}");
                Console.ReadKey();
            }
        }
    }
}