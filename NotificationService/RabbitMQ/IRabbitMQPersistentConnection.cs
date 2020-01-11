using RabbitMQ.Client;

namespace NotificationService.RabbitMQ
{
    public interface IRabbitMQPersistentConnection
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();
        void CreateConsumerChannel(string exchangeName, string routingKey);
        void Disconnect();
    }
}