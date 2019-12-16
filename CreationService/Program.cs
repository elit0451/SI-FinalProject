using System;

namespace CreationService
{
    class Program
    {
        static void Main(string[] args)
        {
            MessageGateway.ReceiveEvent();
            Console.WriteLine("Running");
            while (true) { }
        }
    }
}
