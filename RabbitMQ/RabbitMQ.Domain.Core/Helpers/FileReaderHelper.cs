using System.IO;

namespace RabbitMQ.Domain.Core.Helpers
{
    public static class FileReaderHelper
    {
        public static string Read(string filePath)
        {
            using (var streamReader = new StreamReader(filePath))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}