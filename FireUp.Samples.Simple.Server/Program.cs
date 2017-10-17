using System;

namespace FireUp.Samples.Simple.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Network.Server(51337);
            server.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
