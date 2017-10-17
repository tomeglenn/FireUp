namespace FireUp.Network
{
    public interface IServer
    {
        void Start();
        void Stop();
        void Send(string message, params UdpConnectedClient[] recipients);
        void Broadcast(string message);
    }
}
