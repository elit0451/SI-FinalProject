using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CreationService
{
    public static class MessageGateway
    {
        private static ConnectionFactory factory;
        private static IConnection connection;
        private static IModel channel;
        static MessageGateway()
        {
            factory = new ConnectionFactory() { HostName = "rabbitmq" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        internal static void ReceiveQueue(string exchangeName, string routingKey, EventHandler<BasicDeliverEventArgs> receivedMethod)
        {
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: exchangeName,
                              routingKey: routingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += receivedMethod;

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        internal static void PublishMessage(string exchangeName, string routingKey, string message)
        {
            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);
        }
    }
}