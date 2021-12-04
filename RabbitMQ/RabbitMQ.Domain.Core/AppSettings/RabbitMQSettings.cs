namespace RabbitMQ.Domain.Core.AppSettings
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public uint PrefetchSize { get; set; }
        public ushort PrefetchCount { get; set; }
        public string URL { get; set; }
    }
}