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
        private string _exchangeName;
        private string _routingKey;
        private readonly BlockingCollection<string> rpcResponseQueue = new BlockingCollection<string>();
        private string rpcReplyQueueName;
        private IBasicProperties rpcProperties;
        private EventingBasicConsumer rpcConsumer;
        private IModel rpcChannel;

        private AppDb Db;


        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, AppDb db, string exchangeName, string routingKey)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _exchangeName = exchangeName;
            _routingKey = routingKey;
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
            if (e.RoutingKey == "update")
            {
                var message = Encoding.UTF8.GetString(e.Body);

                JObject receivedObj = JsonConvert.DeserializeObject<JObject>(message);
                int eventId = receivedObj["EventId"].Value<int>();
                string command = receivedObj["Command"].Value<string>().ToLower();
                if (Db.Connection.State != System.Data.ConnectionState.Open)
                    await Db.Connection.OpenAsync();
                EventQuery query = new EventQuery(Db);
                Event evnt = await query.FindOneAsync(eventId);

                if (command == "driver")
                {
                    string driver = receivedObj["DriverName"].Value<string>().ToLower();
                    evnt.DriverName = driver;
                }

                if (Db.Connection.State != System.Data.ConnectionState.Open)
                    await Db.Connection.OpenAsync();
                evnt.Db = Db;
                await evnt.UpdateAsync();
            }
        }

        public void PublishMessage(string exchangeName, string routingKey, string message)
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
                channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
                channel.WaitForConfirmsOrDie();

                channel.BasicAcks += (sender, eventArgs) =>
                {
                    Console.WriteLine("Sent RabbitMQ");
                };
                channel.ConfirmSelect();
            }
        }

        internal string RPCRequest(string exchangeName, string routingKey, string message)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            
            rpcChannel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);
            
            var messageBytes = Encoding.UTF8.GetBytes(message);
            rpcChannel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
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