using System.IO;
using System.Text;

namespace RabbitMQ.Domain.Core.Helpers
{
    public static class FileReaderHelper
    {
        public static string Read(string filePath)
        {
            using var streamReader = new StreamReader(filePath, Encoding.GetEncoding("ISO-8859-1"));
            return streamReader.ReadToEnd();
        }
    }
}