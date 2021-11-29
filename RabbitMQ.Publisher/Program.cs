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
        static void Main(string[] args)
        {
            var workerBase = new WorkerBase(EFila.PublicarPagamento);
            var elmahRepository = workerBase.GetService<IElmahRepository>();

            try
            {
                var settings = workerBase.GetService<Settings>();
                var filasWorkers = workerBase.GetService<FilasWorkersSettings>();
                var rabbit = workerBase.GetService<IRabbitMQBus>();

                Console.WriteLine($"Iniciando Worker {settings.ApplicationName} \n");

                var jsonParaEnvioPath = $@"{AppDomain.CurrentDomain.BaseDirectory}\payload.json";
                var json = "";

                using (var streamReader = new StreamReader(jsonParaEnvioPath))
                {
                    json = streamReader.ReadToEnd();
                }

                Console.WriteLine(json);

                var payload = JsonConvert.DeserializeObject<PublicarPagamentoCommand>(json);

                rabbit.Publicar(payload, filasWorkers.PublicarPagamento);

                Console.WriteLine("\n\nMensagem enviada com sucesso!");
                Console.ReadKey();
            }
            catch(Exception ex)
            {
                elmahRepository.LogarErro(new Error(ex));
                Console.WriteLine($"\n\nErro ao enviar mensagem! {ex.InnerException.Message}");
                Console.ReadKey();
            }
        }
    }
}