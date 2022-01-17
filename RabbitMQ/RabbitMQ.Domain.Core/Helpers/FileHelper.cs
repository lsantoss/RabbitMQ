using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace RabbitMQ.Domain.Core.Helpers
{
    public static class FileHelper
    {
        private static readonly Encoding _encoding = Encoding.GetEncoding("ISO-8859-1");

        public static string Read(string filePath)
        {
            using var streamReader = new StreamReader(filePath, _encoding);
            return streamReader.ReadToEnd();
        }

        public static dynamic ReadJsonToDynamicObject(string filePath)
        {
            using var streamReader = new StreamReader(filePath, _encoding);
            var json = streamReader.ReadToEnd();
            return JsonConvert.DeserializeObject(json);
        }

        public static void SaveObjectToJson(string filePath, object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            using var streamWriter = new StreamWriter(filePath);
            streamWriter.WriteLine(json);
        }
    }
}