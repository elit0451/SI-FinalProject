using RabbitMQ.Client;

namespace NotificationService.RabbitMQ
{
    public interface IRabbitMQPersistentConnection
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();
        void CreateConsumerChannel(string queue);
        void Disconnect();
    }
}