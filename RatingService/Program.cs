using System;

namespace RatingService
{
    class Program
    {
        static void Main(string[] args)
        {
            MessageGateway.ReceiveRating();
            Console.WriteLine("Running");
            while (true) { }
        }
    }
}