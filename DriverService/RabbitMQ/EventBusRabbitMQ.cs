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
        private string _queueName;
        private AppDb Db;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, AppDb db, string queueName = null)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _queueName = queueName;
            Db = db;
        }

        public IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();
            channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += ReceivedEvent;


            channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };
            return channel;
        }

        private async void ReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            if (e.RoutingKey == "driver.find")
            {
                var message = Encoding.UTF8.GetString(e.Body);
                JObject receivedObj = JsonConvert.DeserializeObject<JObject>(message);
                DateTime from = receivedObj["from"].Value<DateTime>();
                DateTime to = receivedObj["to"].Value<DateTime>();

                await Db.Connection.OpenAsync();
                var query = new ApplicationQuery(Db);
                var result = await query.FindAvailableDrivers(new DateTime(2010,06,26), new DateTime(2010,06,27));

                PublishFoundDrivers("driver.found", result);
            }
        }
        public void PublishFoundDrivers(string _queueName, List<Application> drivers)
        {

            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {

                channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                var message = JsonConvert.SerializeObject(drivers);
                var body = Encoding.UTF8.GetBytes(message);

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                channel.ConfirmSelect();
                channel.BasicPublish(exchange: "", routingKey: _queueName, mandatory: true, basicProperties: properties, body: body);
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