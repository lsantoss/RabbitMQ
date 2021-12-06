using System.IO;

namespace RabbitMQ.Domain.Core.Helpers
{
    public static class LeitorArquivosHelper
    {
        public static string Ler(string arquivoPath)
        {
            using (var streamReader = new StreamReader(arquivoPath))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}