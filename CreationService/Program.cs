using System;
using System.Text;
using RabbitMQ.Client.Events;

namespace CreationService
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            MessageGateway.ReceiveQueue("event", "add", ReceivedEvent);
            MessageGateway.ReceiveQueue("cars", "available", ReceivedEvent);
            MessageGateway.ReceiveQueue("driver", "found", ReceivedEvent);

            Console.WriteLine("Running");
            while (true) { }
        }

        void ReceivedEvent(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);

            CommandRouter.Route(message);
        }
    }
}
