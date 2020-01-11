using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RatingService
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

        internal static void ReceiveRating()
        {
            var queueName = channel.QueueDeclare().QueueName;
            channel.ExchangeDeclare(exchange: "event", type: ExchangeType.Direct);

            channel.QueueBind(queue: queueName,
                              exchange: "event",
                              routingKey: "feedback");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                JObject msgObj = JsonConvert.DeserializeObject<JObject>(message);
                msgObj["CorrelationId"] = ea.BasicProperties.CorrelationId;
                msgObj["ReplyTo"] = ea.BasicProperties.ReplyTo;

                await CommandRouter.RouteAsync(msgObj.ToString());

            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        internal static void PublishRPC(string replyTo, string correlationId, string rating)
        {
            var responseBytes = Encoding.UTF8.GetBytes(rating);
            var replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = correlationId;

            channel.BasicPublish(
                exchange: "",
                routingKey: replyTo,
                basicProperties: replyProps,
                body: responseBytes);

            Console.WriteLine("Published to {0}", replyTo);
        }
    }
}