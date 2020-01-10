using System;

namespace LoggerService
{
    class Program
    {
        static void Main(string[] args)
        {
            MessageGateway.ReceiveQueue("logger","info");
            MessageGateway.ReceiveQueue("logger","warning");
            MessageGateway.ReceiveQueue("logger","critical");
            Console.WriteLine("Running");
            while(true)
            {}
        }
    }
}
