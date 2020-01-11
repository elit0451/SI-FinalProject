using System;
using RabbitMQ.Client;

namespace EventService.RabbitMQ
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();

        void CreateConsumerChannel();
        void PublishToChannel(string exchangeName, string routingKey, string message);

        void Disconnect();
        string RPCRequest(string exchangeName, string routingKey, string message);
        void CreateRPCChannel();
    }
}