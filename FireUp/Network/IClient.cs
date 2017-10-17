namespace FireUp.Network
{
    public interface IClient
    {
        void Connect();
        void Disconnect();
        void Send(string message);
    }
}
