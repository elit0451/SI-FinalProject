using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        internal static void ReceiveQueue(string queueName, EventHandler<BasicDeliverEventArgs> receivedMethod)
        {
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += receivedMethod;
                
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