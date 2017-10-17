namespace FireUp.Samples.Json.Core
{
    public class Packet
    {
        public OpCodes OpCode { get; set; }
    }

    public abstract class NumberOperationPacket : Packet
    {
        public int NumberOne { get; set; }
        public int NumberTwo { get; set; }
    }

    public class AdditionPacket : NumberOperationPacket
    {
        public AdditionPacket()
        {
            OpCode = OpCodes.Addition;
        }
    }

    public class SubtractionPacket : NumberOperationPacket
    {
        public SubtractionPacket()
        {
            OpCode = OpCodes.Subtraction;
        }
    }

    public class MultiplicationPacket : NumberOperationPacket
    {
        public MultiplicationPacket()
        {
            OpCode = OpCodes.Multiplication;
        }
    }

    public class MessagePacket : Packet
    {
        public string Message { get; set; }

        public MessagePacket()
        {
            OpCode = OpCodes.Message;
        }
    }
}
