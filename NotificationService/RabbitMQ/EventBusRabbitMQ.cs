using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using NotificationService.Models;

namespace NotificationService.RabbitMQ
{
    public class EventBusRabbitMQ : IDisposable
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private IModel _consumerChannel;
        private AppDb Db;

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, AppDb db)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            Db = db;
        }
        internal IModel CreateConsumerChannel(string queueName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += ReceivedEvent;

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel(queueName);
            };
            return channel;
        }
        private async void ReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            var notificationContent = "";

            switch (e.RoutingKey)
            {
                case "event.add":
                    {
                        var message = Encoding.UTF8.GetString(e.Body);
                        JObject receivedObj = JsonConvert.DeserializeObject<JObject>(message);
                        int eventId = receivedObj["EventId"].Value<int>();
                        string eventType = receivedObj["EventType"]["Name"].Value<string>();
                        string dateFrom = (receivedObj["DateFrom"].Value<DateTime>()).ToString("dd/MM/yyyy");
                        string dateTo = (receivedObj["DateTo"].Value<DateTime>()).ToString("dd/MM/yyyy");
                        string location = receivedObj["Location"].Value<string>();

                        notificationContent = $"{eventType} event was created for {dateFrom} to {dateTo} at {location}";

                        await Db.Connection.OpenAsync();
                        var notification = new Notification()
                        {
                            EventId = eventId,
                            Content = notificationContent,
                            Db = Db
                        };
                        await notification.InsertAsync();
                    }
                    break;

                case "event.update":
                    {
                        var message = Encoding.UTF8.GetString(e.Body);
                        JObject receivedObj = JsonConvert.DeserializeObject<JObject>(message);
                        int eventId = receivedObj["EventId"].Value<int>();
                        string command = receivedObj["Command"].Value<string>().ToLower();

                        if (command == "car")
                        {
                            string license = receivedObj["License"].Value<string>().ToLower();
                            notificationContent = $"Car with license number - {license} was added to the event";
                        }
                        else if (command == "driver")
                        {
                            string driver = receivedObj["DriverName"].Value<string>().ToLower();
                            notificationContent = $"Driver {driver} was assigned to the event";
                        }


                        await Db.Connection.OpenAsync();
                        var notification = new Notification()
                        {
                            EventId = eventId,
                            Content = notificationContent,
                            Db = Db
                        };
                        await notification.InsertAsync();
                    }
                    break;
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