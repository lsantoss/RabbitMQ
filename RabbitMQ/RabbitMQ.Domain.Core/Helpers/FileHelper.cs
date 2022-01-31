using Newtonsoft.Json;
using SelectPdf;
using System;
using System.IO;
using System.Text;

namespace RabbitMQ.Domain.Core.Helpers
{
    public static class FileHelper
    {
        private static readonly Encoding _encoding = Encoding.GetEncoding("ISO-8859-1");
        private static readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string _selectPdfHtmlEngineFullPath = @$"{_basePath}\Select.Html.dep";

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
            using var streamWriter = new StreamWriter(filePath, false, _encoding);
            streamWriter.WriteLine(json);
        }

        public static byte[] ConvertHtmlToPdf(string html)
        {
            GlobalProperties.HtmlEngineFullPath = _selectPdfHtmlEngineFullPath;
            var conversion = new HtmlToPdf().ConvertHtmlString(html);
            var pdfBytes = conversion.Save();
            conversion.Close();
            return pdfBytes;
        }
    }
}