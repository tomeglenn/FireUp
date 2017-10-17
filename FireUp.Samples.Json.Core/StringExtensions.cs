using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FireUp.Samples.Json.Core
{
    public static class StringExtensions
    {
        public static Packet ToPacket(this string s)
        {
            var packetTypeLookup = new Dictionary<OpCodes, Type>
            {
                {OpCodes.Addition, typeof(AdditionPacket)},
                {OpCodes.Subtraction, typeof(SubtractionPacket)},
                {OpCodes.Multiplication, typeof(MultiplicationPacket)},
                {OpCodes.Message, typeof(MessagePacket)}
            };
            
            var packet = JsonConvert.DeserializeObject<Packet>(s);
            return (Packet) JsonConvert.DeserializeObject(s, packetTypeLookup[packet.OpCode]);
        }
    }
}
