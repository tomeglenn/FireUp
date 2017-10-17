using System;
using FireUp.Config;
using FireUp.Network;
using FireUp.Samples.Json.Core;

namespace FireUp.Samples.Json.Server
{
    public class JsonServer : Network.Server
    {
        public JsonServer(int port, ServerConfiguration config = null) : base(port, config) { }

        protected override void OnClientMessageReceived(UdpConnectedClient client, string message)
        {
            var packet = message.ToPacket();

            switch (packet.OpCode)
            {
                case OpCodes.Addition:
                    Add((AdditionPacket)packet);
                    break;
                case OpCodes.Subtraction:
                    Subtract((SubtractionPacket)packet);
                    break;
                case OpCodes.Multiplication:
                    Multiply((MultiplicationPacket)packet);
                    break;
                case OpCodes.Message:
                    Message((MessagePacket)packet);
                    break;
            }
        }

        private void Add(AdditionPacket packet)
        {
            Console.WriteLine(packet.NumberOne + packet.NumberTwo);
        }

        private void Subtract(SubtractionPacket packet)
        {
            Console.WriteLine(packet.NumberOne - packet.NumberTwo);
        }

        private void Multiply(MultiplicationPacket packet)
        {
            Console.WriteLine(packet.NumberOne * packet.NumberTwo);
        }

        private void Message(MessagePacket packet)
        {
            Console.WriteLine(packet.Message);
        }
    }
}
