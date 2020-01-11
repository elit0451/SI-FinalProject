using System;
using System.Collections.Generic;
using System.Text;
using DriverService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DriverService.RabbitMQ
{
    public class EventBusRabbitMQ : IDisposable
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private IModel _consumerChannel;
        private string _exchangeName;
        private string _routingKey;
        private AppDb Db;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, AppDb db, string exchangeName, string routingKey)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _exchangeName = exchangeName;
            _routingKey = routingKey;
            Db = db;
        }

        public IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct);

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName,
                              exchange: _exchangeName,
                              routingKey: _routingKey);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += ReceivedEvent;

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };
            return channel;
        }

        private async void ReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            if (e.RoutingKey == "find")
            {
                var message = Encoding.UTF8.GetString(e.Body);
                JObject receivedObj = JsonConvert.DeserializeObject<JObject>(message);
                DateTime from = receivedObj["From"].Value<DateTime>();
                DateTime to = receivedObj["To"].Value<DateTime>();
                string requestId = receivedObj["RequestId"].Value<string>();

                if (Db.Connection.State != System.Data.ConnectionState.Open)
                    await Db.Connection.OpenAsync();
                var query = new ApplicationQuery(Db);
                var result = await query.FindAvailableDrivers(from, to);

                JObject reply = new JObject();
                reply.Add("RequestId", requestId);
                reply.Add("Command", "driverFound");
                reply.Add("Drivers", JToken.FromObject(result));

                Booking booking = new Booking(Db);
                booking.ApplicationId = result[0].ApplicationID;
                booking.FromTime = from;
                booking.ToTime = to;
                await booking.InsertAsync();

                PublishFoundDrivers("driver", "found", reply.ToString());
            }
        }
        public void PublishFoundDrivers(string exchangeName, string routingKey, string message)
        {

            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {

                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

                var body = Encoding.UTF8.GetBytes(message);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                channel.ConfirmSelect();
                channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, mandatory: true, basicProperties: properties, body: body);
                channel.WaitForConfirmsOrDie();

                channel.BasicAcks += (sender, eventArgs) =>
                {
                    Console.WriteLine("Sent RabbitMQ");
                    //implement ack handle
                };
                channel.ConfirmSelect();
            }
        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
        }
    }
}