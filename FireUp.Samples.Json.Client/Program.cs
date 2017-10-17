using System;
using FireUp.Samples.Json.Core;

namespace FireUp.Samples.Json.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new JsonClient("localhost", 51337);
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
                        Packet packet = new MessagePacket { Message = input };

                        var data = input.Split();
                        if (data.Length == 3)
                        {
                            var a = int.Parse(data[1]);
                            var b = int.Parse(data[2]);

                            if (data[0] == "add")
                            {
                                packet = new AdditionPacket {NumberOne = a, NumberTwo = b};
                            }

                            if (data[0] == "subtract")
                            {
                                packet = new SubtractionPacket() { NumberOne = a, NumberTwo = b };
                            }

                            if (data[0] == "multiply")
                            {
                                packet = new MultiplicationPacket() { NumberOne = a, NumberTwo = b };
                            }
                        }

                        client.Send(packet);
                        break;
                }
            }

            client.Disconnect();
        }
    }
}
