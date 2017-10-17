using System;
using System.Net;

namespace FireUp.Network
{
    public class UdpConnectedClient
    {
        public IPEndPoint Endpoint { get; }
        public DateTime LastMessageReceived { get; private set; }

        public UdpConnectedClient(IPEndPoint endpoint)
        {
            Endpoint = endpoint;
            LastMessageReceived = DateTime.UtcNow;
        }

        public void KeepAlive()
        {
            LastMessageReceived = DateTime.UtcNow;
        }

        public bool HasTimedOut(int minimumIntervalInSeconds)
        {
            return DateTime.UtcNow - LastMessageReceived >= TimeSpan.FromSeconds(minimumIntervalInSeconds);
        }

        public override string ToString()
        {
            return $"{Endpoint.Address}:{Endpoint.Port}";
        }
    }
}
