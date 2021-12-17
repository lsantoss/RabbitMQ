using Newtonsoft.Json;

namespace RabbitMQ.Domain.Core.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveJsonFormatting(this string json)
        {
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json));
        }
    }
}