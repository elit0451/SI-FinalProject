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
        void PublishToChannel(string channelName, string message);

        void Disconnect();
        string RPCRequest(string queueName, string message);
        void CreateRPCChannel();
    }
}