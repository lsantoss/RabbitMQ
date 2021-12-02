namespace RabbitMQ.Domain.Core.RabbitMQ.Interfaces
{
    public interface IRabbitMQBus
    {
        void Publicar<T>(T @event, string nomeFila, bool duravel = true, bool excluivel = false, bool apagaAutomaticamente = false);
        void PublicarComAtraso<T>(T @event, string nomeFila, bool duravel = true, bool excluivel = false, bool apagaAutomaticamente = false, int tempoAtraso = 15000);
        void Consumir(object handler, string nomeFila, bool duravel = true, bool excluivel = false, bool apagaAutomaticamente = false);
        void Consumir<T>(object handler, string nomeFila, bool duravel = true, bool excluivel = false, bool apagaAutomaticamente = false);
    }
}