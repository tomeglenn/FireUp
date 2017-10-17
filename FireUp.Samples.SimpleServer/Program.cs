using System;
using FireUp.Network;

namespace FireUp.Samples.SimpleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server(51337);
            server.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
