using System;

namespace FireUp.Samples.Json.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new JsonServer(51337);
            server.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
