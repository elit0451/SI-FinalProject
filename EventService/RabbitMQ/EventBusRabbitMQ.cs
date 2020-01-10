using System;
using System.Collections.Concurrent;
using System.Text;
using EventService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventService.RabbitMQ
{
    public class EventBusRabbitMQ : IDisposable
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private IModel _consumerChannel;
        private string _queueName;
        private readonly BlockingCollection<string> rpcResponseQueue = new BlockingCollection<string>();
        private string rpcReplyQueueName;
        private IBasicProperties rpcProperties;
        private EventingBasicConsumer rpcConsumer;
        private IModel rpcChannel;

        private AppDb Db;


        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, AppDb db, string queueName = null)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _queueName = queueName;
            Db = db;
        }

        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
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

        internal void CreateRPCChannel()
        {

            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            rpcChannel = _persistentConnection.CreateModel();
            rpcReplyQueueName = rpcChannel.QueueDeclare().QueueName;
            rpcConsumer = new EventingBasicConsumer(rpcChannel);

            rpcProperties = rpcChannel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            rpcProperties.CorrelationId = correlationId;
            rpcProperties.ReplyTo = rpcReplyQueueName;

            rpcConsumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    rpcResponseQueue.Add(response);
                }
            };
        }

        private async void ReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            if (e.RoutingKey == "event.update")
            {
                var message = Encoding.UTF8.GetString(e.Body);

                JObject receivedObj = JsonConvert.DeserializeObject<JObject>(message);
                int eventId = receivedObj["EventId"].Value<int>();
                string command = receivedObj["Command"].Value<string>().ToLower();

                EventQuery query = new EventQuery(Db);
                Event evnt = await query.FindOneAsync(eventId);

                if (command == "driver")
                {
                    string driver = receivedObj["DriverName"].Value<string>().ToLower();
                    evnt.DriverName = driver;
                }

                await Db.Connection.OpenAsync();
                evnt.Db = Db;
                await evnt.UpdateAsync();
            }
        }

        public void PublishMessage(string _queueName, string message)
        {

            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using (var channel = _persistentConnection.CreateModel())
            {

                channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

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
                };
                channel.ConfirmSelect();
            }
        }

        internal string RPCRequest(string channelName, string message)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var messageBytes = Encoding.UTF8.GetBytes(message);
            rpcChannel.BasicPublish(
                exchange: "",
                routingKey: channelName,
                mandatory: true,
                basicProperties: rpcProperties,
                body: messageBytes);

            rpcChannel.BasicConsume(
                consumer: rpcConsumer,
                queue: rpcReplyQueueName,
                autoAck: true);


            return rpcResponseQueue.Take();
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