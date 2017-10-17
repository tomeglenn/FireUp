using System;
using FireUp.Network;

namespace FireUp.Samples.SimpleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client("localhost", 51337);
            client.Connect();

            Console.WriteLine("Type 'exit' to exit");

            var quit = false;
            while (!quit)
            {
                var input = Console.ReadLine() ?? "";
                switch (input.ToLower())
                {
                    case "exit":
                        quit = true;
                        break;
                    default:
                        client.Send(input);
                        break;
                }
            }

            client.Disconnect();
        }
    }
}
