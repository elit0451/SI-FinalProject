using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LoggerService
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

        internal static void ReceiveQueue(string exchangeName, string routingKey)
        {
            channel.ExchangeDeclare(exchange: exchangeName,
                                    type: "direct");

            string queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName,
                                  exchange: exchangeName,
                                  routingKey: routingKey);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                CommandRouter.Route(ea.RoutingKey, message);
            };


            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        internal static void PublishMessage(string queueName, string message)
        {

            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);


            var body = Encoding.UTF8.GetBytes(message);

            Console.WriteLine("Publishing to: " + queueName + " - Message: \n" + message);
            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body);
        }
    }
}